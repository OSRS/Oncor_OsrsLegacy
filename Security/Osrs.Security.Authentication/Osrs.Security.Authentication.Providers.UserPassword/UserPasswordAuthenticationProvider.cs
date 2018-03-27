//Copyright 2017 Open Science, Engineering, Research and Development Information Systems Open, LLC. (OSRS Open)
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Osrs.Security.Identity;
using Osrs.Security.Passwords;
using System;
using System.Collections.Generic;

namespace Osrs.Security.Authentication.Providers
{
    //TODO -- add system binding
    public class UserPasswordAuthenticationProvider : IAuthenticationProvider
    {
        private readonly UserSecurityContext Context;
        private readonly ICredentialStore storeProvider;
        private readonly UserPasswordHistoryProvider historyProvider;

        public bool AddCredential(IUserIdentity user, Security.ICredential credential)
        {
            if (user!=null && credential!=null)
            {
                UserPasswordCredential up = credential as UserPasswordCredential; //this is the only type of credential we can use
                if (up != null)
                {
                    UsernamePassword hist = up.ToHistoryPair(); //this is what we store in history
                    if (UserPasswordProviderFactory.Instance.ComplexityChecker.IsValid(up.password)) //validate it meets complexity rules
                    {
                        //get the existing credential this will replace, or this may be all together new
                        PersistedCredential curCred = GetPersisted(user.Uid, hist.UserName);
                        if (curCred != null)
                        {
                            PasswordHistory<UsernamePassword> history = GetHistory(user.Uid, hist.UserName);
                            if (!history.MatchesHistory(UsernamePassword.Matches, hist)) //can't add this since it's matching a current history in range
                            {
                                history.Add(hist);
                                //delete and set the new password
                                if (ReplaceCredential(user.Uid, hist.UserName, hist.Password))
                                {
                                    this.historyProvider.Update(user.Uid, history);
                                    return true;
                                }
                            }
                        }
                        else //no existing cred, so this is a new one - but already meets complexity rule
                        {
                            PasswordHistory<UsernamePassword> history = GetHistory(user.Uid, hist.UserName);
                            if (!history.MatchesHistory(UsernamePassword.Matches, hist)) //can't add this since it's matching a current history in range
                            {
                                history.Add(hist);
                                //delete and set the new password
                                if (this.storeProvider.AddCredential(user.Uid, this, hist.UserName, hist.Password)) //the hist is already salted
                                {
                                    this.historyProvider.Update(user.Uid, history);  //update the history
                                    return true;
                                }
                            }
                        }
                    } //inside this level we have a valid password by complexity
                }
            }
            return false;
        }

        public IUserIdentity Authenticate(Security.ICredential credential)
        {
            if (credential!=null)
            {
                UserPasswordCredential up = credential as UserPasswordCredential;
                if (up!=null)
                {
                    IEnumerable<PersistedCredential> creds = this.storeProvider.Get(this, up.UserName.ToLowerInvariant());
                    if (creds!=null)
                    {
                        foreach(PersistedCredential cur in creds)
                        {
                            if (up.Matches(cur))
                            {
                                return IdentityManager.Instance.GetProvider(this.Context).Get(cur.UserId);
                            }
                        }
                    }
                }
            }
            return null;
        }

        public bool Authenticates(Security.ICredential credential)
        {
            if (credential != null)
            {
                UserPasswordCredential up = credential as UserPasswordCredential;
                if (up != null)
                {
                    IEnumerable<PersistedCredential> creds = this.storeProvider.Get(this, up.UserName.ToLowerInvariant());
                    if (creds != null)
                    {
                        foreach (PersistedCredential cur in creds)
                        {
                            if (up.Matches(cur))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool CanAuthenticate(Security.ICredential credential)
        {
            if (credential != null)
                return (credential as UserPasswordCredential) != null;
            return false;
        }

        public bool DeleteCredential(IUserIdentity user, Security.ICredential credential)
        {
            if (user!=null && credential!=null)
            {
                UserPasswordCredential toRemove = credential as UserPasswordCredential; //this is the only type of credential we can use
                if (toRemove != null)
                {
                    return this.storeProvider.DeleteCredential(user.Uid, this, toRemove.UserName.ToLowerInvariant());
                }
            }
            return false;
        }

        public bool ReplaceCredential(IUserIdentity user, Security.ICredential existing, Security.ICredential proposed)
        {
            if (user != null && existing != null && proposed != null)
            {
                UserPasswordCredential toRemove = existing as UserPasswordCredential; //this is the only type of credential we can use
                UserPasswordCredential toAdd = proposed as UserPasswordCredential; //this is the only type of credential we can use
                if (toRemove != null && toAdd!=null)
                {
                    PersistedCredential cred = GetPersisted(user.Uid, toRemove.UserName.ToLowerInvariant());
                    if (cred!=null && toRemove.Matches(cred)) //has to be currently valid to do a replace
                    {
                        UsernamePassword up = toAdd.ToHistoryPair();
                        if (toRemove.UserName.ToLowerInvariant().Equals(toAdd.UserName.ToLowerInvariant()))
                        {
                            PasswordHistory<UsernamePassword> hist = GetHistory(user.Uid, up.UserName);
                            if (!hist.MatchesHistory(UsernamePassword.Matches, up)) //check for historic collision
                            {
                                if (ReplaceCredential(user.Uid, up.UserName, up.Password))
                                {
                                    hist.Add(up);
                                    this.historyProvider.Update(user.Uid, hist);
                                    return true;
                                }
                            }
                        }
                        else //different usernames being used
                        {
                            if (this.storeProvider.DeleteCredential(user.Uid, this, toRemove.UserName.ToLowerInvariant()))
                            {
                                this.historyProvider.Delete(user.Uid); //TODO -- this we need to fix by userId,type
                                if (this.storeProvider.AddCredential(user.Uid, this, up.UserName, up.Password))
                                {
                                    PasswordHistory<UsernamePassword> hist = GetHistory(user.Uid, up.UserName);
                                    this.historyProvider.Update(user.Uid, hist);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool ReplaceCredential(Guid userId, string username, string saltedPasswordHash)
        {
            if (this.storeProvider.DeleteCredential(userId, this, username))
            {
                return this.storeProvider.AddCredential(userId, this, username, saltedPasswordHash);
            }
            return false;
        }

        private PersistedCredential GetPersisted(Guid userId, string userNameLowered)
        {
            IEnumerable<PersistedCredential> creds = this.storeProvider.Get(this, userId, userNameLowered);
            if (creds != null)
            {
                foreach (PersistedCredential curCred in creds)
                {
                    if (curCred.TextToken.Equals(userNameLowered)) //this is the existing credential we'd replace
                        return curCred;
                }
            }

            return null;
        }

        private PasswordHistory<UsernamePassword> GetHistory(Guid userId, string userName)
        {
            IEnumerable<PasswordHistory<UsernamePassword>> history = this.historyProvider.Get(userId);
            if (history != null)
            {
                foreach (PasswordHistory<UsernamePassword> cur in history)
                {
                    if (cur[0].UserName.Equals(userName, StringComparison.Ordinal)) //already case matched
                        return cur;
                }
            }
            //no history, or unmatched user name for user id, maybe configuration changed
            PasswordHistory<UsernamePassword> curHist = new PasswordHistory<UsernamePassword>();
            curHist.MaxHistory = UserPasswordProviderFactory.Instance.MaxHistory;
            return curHist;
        }

        internal UserPasswordAuthenticationProvider(UserSecurityContext context, ICredentialStore storeProvider, UserPasswordHistoryProvider histProv)
        {
            this.Context = context;
            this.storeProvider = storeProvider;
            this.historyProvider = histProv;
        }
    }
}
