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

using Osrs.Numerics.Spatial.Coordinates;
using System;
using System.Collections.Generic;
using System.IO;

namespace Osrs.Numerics.Spatial.Geometry
{
    /// <summary>  
    /// Converts a Well-Known Text string to a <c>Geometry</c>.
    /// 
    /// The <c>WKTReader</c> allows
    /// extracting <c>Geometry</c> objects from either input streams or
    /// internal strings. This allows it to function as a parser to read <c>Geometry</c>
    /// objects from text blocks embedded in other data formats (e.g. XML). 
    /// 
    /// The Well-known
    /// Text format is defined in the <A HREF="http://www.opengis.org/techno/specs.htm">
    /// OpenGIS Simple Features Specification for SQL</A> . 
    /// 
    /// NOTE:  There is an inconsistency in the SFS. 
    /// The WKT grammar states that <c>MultiPoints</c> are represented by 
    /// <c>MULTIPOINT ( ( x y), (x y) )</c>, 
    /// but the examples show <c>MultiPoint</c>s as <c>MULTIPOINT ( x y, x y )</c>. 
    /// Other implementations follow the latter syntax, so NTS will adopt it as well.
    /// A <c>WKTReader</c> is parameterized by a <c>GeometryFactory</c>, 
    /// to allow it to create <c>Geometry</c> objects of the appropriate
    /// implementation. In particular, the <c>GeometryFactory</c> will
    /// determine the <c>PrecisionModel</c> and <c>SRID</c> that is used. 
    /// The <c>WKTReader</c> will convert the input numbers to the precise
    /// internal representation.
    /// <remarks>
    /// <see cref="WKTReader" /> reads also non-standard "LINEARRING" tags.
    /// </remarks>
    /// </summary>
    public sealed class WKTReader
    {
        private GeometryFactory2Double factory = GeometryFactory2Double.Instance;

        private static readonly System.Globalization.CultureInfo InvariantCulture = System.Globalization.CultureInfo.InvariantCulture;
        private static readonly string NaNString = double.NaN.ToString(InvariantCulture); /*"NaN"*/

        /// <summary> 
        /// Creates a <c>WKTReader</c> that creates objects using a basic GeometryFactory.
        /// </summary>
        public WKTReader()
        { }

        /// <summary>  
        /// Creates a <c>WKTReader</c> that creates objects using the given
        /// <c>GeometryFactory</c>.
        /// </summary>
        /// <param name="geometryFactory">The factory used to create <c>Geometry</c>s.</param>
        public WKTReader(IGeometryFactory geometryFactory)
        { }

        /// <summary>
        /// Gets or sets the factory to create geometries
        /// </summary>
        public IGeometryFactory<double> Factory
        {
            get { return factory; }
        }

        /// <summary>
        /// Gets or sets the default SRID
        /// </summary>
        public int DefaultSRID { get; set; }

        /// <summary>
        /// Converts a Well-known Text representation to a <c>Geometry</c>.
        /// </summary>
        /// <param name="wellKnownText">
        /// one or more Geometry Tagged Text strings (see the OpenGIS
        /// Simple Features Specification) separated by whitespace.
        /// </param>
        /// <returns>
        /// A <c>Geometry</c> specified by <c>wellKnownText</c>
        /// </returns>
        public IGeometry Read(string wellKnownText)
        {
            using (var reader = new StringReader(wellKnownText))
            {
                return Read(reader);
            }
        }

        /// <summary>
        /// Converts a Well-known Text representation to a <c>Geometry</c>.
        /// </summary>
        /// <param name="stream">
        /// one or more Geometry Tagged Text strings (see the OpenGIS
        /// Simple Features Specification) separated by whitespace.
        /// </param>
        /// <returns>
        /// A <c>Geometry</c> specified by <c>wellKnownText</c>
        /// </returns>
        public IGeometry Read(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return Read(reader);
            }
        }

        /// <summary>  
        /// Converts a Well-known Text representation to a <c>Geometry</c>.
        /// </summary>
        /// <param name="reader"> 
        /// A Reader which will return a "Geometry Tagged Text"
        /// string (see the OpenGIS Simple Features Specification).
        /// </param>
        /// <returns>A <c>Geometry</c> read from <c>reader</c>.
        /// </returns>
        public IGeometry Read(TextReader reader)
        {
            /*
            var tokens = Tokenize(reader);
            StreamTokenizer tokenizer = new StreamTokenizer(reader);
            IList<Token> tokens = new List<Token>();
            tokenizer.Tokenize(tokens);     // Read directly all tokens
             */
            //_index = 0;                      // Reset pointer to start of tokens
            try
            {
                var enumerator = new StreamTokenizer(reader).GetEnumerator();
                enumerator.MoveNext();
                return ReadGeometryTaggedText(enumerator);
            }
            catch (IOException e)
            {
                throw new ParseException(e.ToString());
            }
        }

        internal IEnumerator<Token> Tokenizer(TextReader reader)
        {
            return new StreamTokenizer(reader).GetEnumerator();
        }

        internal IList<Token> Tokenize(TextReader reader)
        {
            var tokenizer = new StreamTokenizer(reader);
            IList<Token> tokens = new List<Token>();
            tokenizer.Tokenize(tokens);     // Read directly all tokens
            return tokens;
        }

        //internal int Index { get { return _index; } set { _index = value; } }

        /// <summary>
        /// Returns the next array of <c>Coordinate</c>s in the stream.
        /// </summary>
        /// <param name="tokens">
        /// Tokenizer over a stream of text in Well-known Text
        /// format. The next element returned by the stream should be "(" (the
        /// beginning of "(x1 y1, x2 y2, ..., xn yn)") or "EMPTY".
        /// </param>
        /// <param name="skipExtraParenthesis">
        /// if set to <c>true</c> skip extra parenthesis around coordinates.
        /// </param>
        /// <returns>
        /// The next array of <c>Coordinate</c>s in the
        /// stream, or an empty array if "EMPTY" is the next element returned by
        /// the stream.
        /// </returns>
        private Point2<double>[] GetCoordinates(IEnumerator<Token> tokens, Boolean skipExtraParenthesis)
        {
            string nextToken = GetNextEmptyOrOpener(tokens);
            if (nextToken.Equals("EMPTY"))
                return new Point2<double>[] { };
            var coordinates = new List<Point2<double>>();
            coordinates.Add(factory.ConstructPoint(GetPreciseCoordinate(tokens, skipExtraParenthesis)));
            nextToken = GetNextCloserOrComma(tokens);
            while (nextToken.Equals(","))
            {
                coordinates.Add(factory.ConstructPoint(GetPreciseCoordinate(tokens, skipExtraParenthesis)));
                nextToken = GetNextCloserOrComma(tokens);
            }
            return coordinates.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="skipExtraParenthesis"></param>
        /// <returns></returns>
        private Coordinate2<double> GetPreciseCoordinate(IEnumerator<Token> tokens, Boolean skipExtraParenthesis)
        {
            var coord = new Coordinate2<double>();
            var extraParenthesisFound = false;
            if (skipExtraParenthesis)
            {
                extraParenthesisFound = IsStringValueNext(tokens, "(");
                if (extraParenthesisFound)
                {
                    tokens.MoveNext();
                    //_index++;
                }
            }
            coord.X = GetNextNumber(tokens);
            coord.Y = GetNextNumber(tokens);
            if (IsNumberNext(tokens))
            {
                GetNextNumber(tokens); //skip over Z
            }

            if (skipExtraParenthesis &&
                extraParenthesisFound &&
                IsStringValueNext(tokens, ")"))
            {
                tokens.MoveNext();
                //_index++;
            }

            return coord;
        }

        private static Boolean IsStringValueNext(IEnumerator<Token> tokens, String stringValue)
        {
            var token = tokens.Current /*as Token*/;
            if (token == null)
                throw new InvalidOperationException("current Token is null");
            return token.StringValue == stringValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private static bool IsNumberNext(IEnumerator<Token> tokens)
        {
            var token = tokens.Current /*as Token*/;
            return token is FloatToken ||
                   token is IntToken ||
                   (token is WordToken && string.Compare(token.Object.ToString(), NaNString, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// Returns the next number in the stream.
        /// </summary>
        /// <param name="tokens">
        /// Tokenizer over a stream of text in Well-known Text
        /// format. The next token must be a number.
        /// </param>
        /// <returns>The next number in the stream.</returns>
        /// <exception cref="GeoAPI.IO.ParseException">if the next token is not a valid number</exception>
        private static double GetNextNumber(IEnumerator<Token> tokens)
        {
            var token = tokens.Current /*as Token*/;
            if (!tokens.MoveNext())
                throw new InvalidOperationException("premature end of enumerator");

            if (token == null)
                throw new ArgumentNullException("tokens", "Token list contains a null value");
            if (token is EofToken)
                throw new ParseException("Expected number but encountered end of stream");
            if (token is EolToken)
                throw new ParseException("Expected number but encountered end of line");
            if (token is FloatToken || token is IntToken)
                return (double)token.ConvertToType(typeof(double));
            if (token is WordToken)
            {
                if (string.Compare(token.Object.ToString(), NaNString, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return Double.NaN;
                }
                throw new ParseException("Expected number but encountered word: " + token.StringValue);
            }
            if (token.StringValue == "(")
                throw new ParseException("Expected number but encountered '('");
            if (token.StringValue == ")")
                throw new ParseException("Expected number but encountered ')'");
            if (token.StringValue == ",")
                throw new ParseException("Expected number but encountered ','");

            throw new ParseException("Expected number but encountered '" + token.StringValue + "'");
        }

        /// <summary>
        /// Returns the next "EMPTY" or "(" in the stream as uppercase text.
        /// </summary>
        /// <param name="tokens">
        /// Tokenizer over a stream of text in Well-known Text
        /// format. The next token must be "EMPTY" or "(".
        /// </param>
        /// <returns>
        /// The next "EMPTY" or "(" in the stream as uppercase text.</returns>
        private static string GetNextEmptyOrOpener(IEnumerator<Token> tokens)
        {
            string nextWord = GetNextWord(tokens);
            if (nextWord.Equals("EMPTY") || nextWord.Equals("("))
                return nextWord;
            throw new ParseException("Expected 'EMPTY' or '(' but encountered '" + nextWord + "'");
        }

        /// <summary>
        /// Returns the next ")" or "," in the stream.
        /// </summary>
        /// <param name="tokens">
        /// Tokenizer over a stream of text in Well-known Text
        /// format. The next token must be ")" or ",".
        /// </param>
        /// <returns>
        /// The next ")" or "," in the stream.</returns>
        private static string GetNextCloserOrComma(IEnumerator<Token> tokens)
        {
            string nextWord = GetNextWord(tokens);
            if (nextWord.Equals(",") || nextWord.Equals(")"))
                return nextWord;

            throw new ParseException("Expected ')' or ',' but encountered '" + nextWord + "'");
        }

        /// <summary>
        /// Returns the next ")" in the stream.
        /// </summary>
        /// <param name="tokens">
        /// Tokenizer over a stream of text in Well-known Text
        /// format. The next token must be ")".
        /// </param>
        /// <returns>
        /// The next ")" in the stream.</returns>
        private static string GetNextCloser(IEnumerator<Token> tokens)
        {
            var nextWord = GetNextWord(tokens);
            if (nextWord.Equals(")"))
                return nextWord;
            throw new ParseException("Expected ')' but encountered '" + nextWord + "'");
        }

        /// <summary>
        /// Returns the next word in the stream as uppercase text.
        /// </summary>
        /// <param name="tokens">
        /// Tokenizer over a stream of text in Well-known Text
        /// format. The next token must be a word.
        /// </param>
        /// <returns>The next word in the stream as uppercase text.</returns>
        private static string GetNextWord(IEnumerator<Token> tokens)
        {
            var token = tokens.Current /*as Token*/;
            if (token == null)
                throw new InvalidOperationException("current token is null");

            if (!tokens.MoveNext())
                throw new InvalidOperationException("premature end of enumerator");

            if (token is EofToken)
                throw new ParseException("Expected number but encountered end of stream");
            if (token is EolToken)
                throw new ParseException("Expected number but encountered end of line");
            if (token is FloatToken || token is IntToken)
                throw new ParseException("Expected word but encountered number: " + token.StringValue);
            if (token is WordToken)
                return token.StringValue.ToUpper();
            if (token.StringValue == "(")
                return "(";
            if (token.StringValue == ")")
                return ")";
            if (token.StringValue == ",")
                return ",";

            throw new InvalidOperationException("Should never reach here!");
            //Assert.ShouldNeverReachHere();
            //return null;
        }

        /// <summary>
        /// Creates a <c>Geometry</c> using the next token in the stream.
        /// </summary>
        /// <param name="tokens">
        /// Tokenizer over a stream of text in Well-known Text
        /// format. The next tokens must form a &lt;Geometry Tagged Text.
        /// </param>
        /// <returns>A <c>Geometry</c> specified by the next token
        /// in the stream.</returns>
        internal IGeometry2<double> ReadGeometryTaggedText(IEnumerator<Token> tokens)
        {
            /*
             * A new different implementation by Marc Jacquin:
             * this code manages also SRID values.
             */
            IGeometry2<double> returned;

            int srid;
            var type = GetNextWord(tokens);
            if (type == "SRID")
            {
                tokens.MoveNext(); // =
                GetNextNumber(tokens);
                tokens.MoveNext(); // ;
                type = GetNextWord(tokens);
            }
            else
                srid = DefaultSRID;

            /*Test of Z, M or ZM suffix*/
            var suffix = tokens.Current;

            if (suffix is WordToken)
            {
                if (suffix == "Z")
                {
                    tokens.MoveNext();
                }
                else if (suffix == "ZM")
                {
                    tokens.MoveNext();
                }
                else if (suffix == "M")
                {
                    tokens.MoveNext();
                }
            }

            if (type.Equals("POINT"))
                returned = ReadPointText(tokens);
            else if (type.Equals("LINESTRING"))
                returned = ReadLineStringText(tokens);
            else if (type.Equals("LINEARRING"))
                returned = ReadLinearRingText(tokens);
            else if (type.Equals("POLYGON"))
                returned = ReadPolygonText(tokens);
            else if (type.Equals("MULTIPOINT"))
                returned = ReadMultiPointText(tokens);
            else if (type.Equals("MULTILINESTRING"))
                returned = ReadMultiLineStringText(tokens);
            else if (type.Equals("MULTIPOLYGON"))
                returned = ReadMultiPolygonText(tokens);
            //else if (type.Equals("GEOMETRYCOLLECTION"))
            //    returned = ReadGeometryCollectionText(tokens);
            else throw new ParseException("Unknown type: " + type);

            if (returned == null)
                throw new NullReferenceException("Error reading geometry");

            return returned;
        }

        /// <summary>
        /// Creates a <c>Point</c> using the next token in the stream.
        /// </summary>
        /// <param name="tokens">
        ///   Tokenizer over a stream of text in Well-known Text
        ///   format. The next tokens must form a &lt;Point Text.
        /// </param>
        /// <param name="factory"> </param>
        /// <returns>A <c>Point</c> specified by the next token in
        /// the stream.</returns>
        private Point2<double> ReadPointText(IEnumerator<Token> tokens)
        {
            var nextToken = GetNextEmptyOrOpener(tokens);
            if (nextToken.Equals("EMPTY"))
                return null;
            var coord = GetPreciseCoordinate(tokens, false);
            var point = factory.ConstructPoint(coord);
            GetNextCloser(tokens);
            return point;
        }

        private PointBag2<double> ToSequence(bool hasZ, params Coordinate2<double>[] coords)
        {
            List<Point2<double>> pts = new List<Point2<double>>(coords.Length);
            for (var i = 0; i < coords.Length; i++)
            {
                pts.Add(factory.ConstructPoint(coords[i]));
            }
            return factory.ConstructPointBag(pts);
        }

        /// <summary>
        /// Creates a <c>LineString</c> using the next token in the stream.
        /// </summary>
        /// <param name="tokens">
        ///   Tokenizer over a stream of text in Well-known Text
        ///   format. The next tokens must form a &lt;LineString Text.
        /// </param>
        /// <param name="factory"> </param>
        /// <returns>
        /// A <c>LineString</c> specified by the next
        /// token in the stream.</returns>
        private Polyline2<double> ReadLineStringText(IEnumerator<Token> tokens)
        {
            var coords = GetCoordinates(tokens, false);
            return factory.ConstructPolyline(coords);
        }

        /// <summary>
        /// Creates a <c>LinearRing</c> using the next token in the stream.
        /// </summary>
        /// <param name="tokens">
        ///   Tokenizer over a stream of text in Well-known Text
        ///   format. The next tokens must form a &lt;LineString Text.
        /// </param>
        /// <param name="factory"> </param>
        /// <returns>A <c>LinearRing</c> specified by the next
        /// token in the stream.</returns>
        private Ring2<double> ReadLinearRingText(IEnumerator<Token> tokens)
        {
            var coords = GetCoordinates(tokens, false);
            return factory.ConstructRing(coords);
        }

        /// <summary>
        /// Creates a <c>MultiPoint</c> using the next token in the stream.
        /// </summary>
        /// <param name="tokens">
        ///   Tokenizer over a stream of text in Well-known Text
        ///   format. The next tokens must form a &lt;MultiPoint Text.
        /// </param>
        /// <param name="factory"> </param>
        /// <returns>
        /// A <c>MultiPoint</c> specified by the next
        /// token in the stream.</returns>
        private PointBag2<double> ReadMultiPointText(IEnumerator<Token> tokens)
        {
            var coords = GetCoordinates(tokens, true);
            return factory.ConstructPointBag(coords);
        }

        /// <summary> 
        /// Creates an array of <c>Point</c>s having the given <c>Coordinate</c>s.
        /// </summary>
        /// <param name="coordinates">
        /// The <c>Coordinate</c>s with which to create the <c>Point</c>s
        /// </param>
        /// <param name="factory">The factory to create the points</param>
        /// <returns>
        /// <c>Point</c>s created using this <c>WKTReader</c>
        /// s <c>GeometryFactory</c>.
        /// </returns>
        private Point2<double>[] ToPoints(Coordinate2<double>[] coordinates)
        {
            var points = new Point2<double>[coordinates.Length];
            for (var i = 0; i < coordinates.Length; i++)
            {
                points[i] = factory.ConstructPoint(coordinates[i]);
            }
            return points;
        }

        /// <summary>  
        /// Creates a <c>Polygon</c> using the next token in the stream.
        /// </summary>
        /// <param name="tokens">
        ///   Tokenizer over a stream of text in Well-known Text
        ///   format. The next tokens must form a Polygon Text.
        /// </param>
        /// <param name="factory"> </param>
        /// <returns>
        /// A <c>Polygon</c> specified by the next token
        /// in the stream.        
        /// </returns>
        private Polygon2<double> ReadPolygonText(IEnumerator<Token> tokens)
        {
            string nextToken = GetNextEmptyOrOpener(tokens);
            if (nextToken.Equals("EMPTY"))
                return null;

            var holes = new List<Ring2<double>>();
            var shell = ReadLinearRingText(tokens);
            nextToken = GetNextCloserOrComma(tokens);
            while (nextToken.Equals(","))
            {
                Ring2<double> hole = ReadLinearRingText(tokens);
                holes.Add(hole);
                nextToken = GetNextCloserOrComma(tokens);
            }
            if (holes.Count < 1)
                return factory.ConstructPolygon(shell);
            return factory.ConstructPolygon(shell, factory.ConstructRingSet(holes));
        }

        /// <summary>
        /// Creates a <c>MultiLineString</c> using the next token in the stream.
        /// </summary>
        /// <param name="tokens">
        ///   Tokenizer over a stream of text in Well-known Text
        ///   format. The next tokens must form a MultiLineString Text.
        /// </param>
        /// <param name="factory"> </param>
        /// <returns>
        /// A <c>MultiLineString</c> specified by the
        /// next token in the stream.</returns>
        private PolylineBag2<double> ReadMultiLineStringText(IEnumerator<Token> tokens)
        {
            string nextToken = GetNextEmptyOrOpener(tokens);
            if (nextToken.Equals("EMPTY"))
                return null;

            var lineStrings = new List<Polyline2<double>>();
            var lineString = ReadLineStringText(tokens);
            lineStrings.Add(lineString);
            nextToken = GetNextCloserOrComma(tokens);
            while (nextToken.Equals(","))
            {

                lineString = ReadLineStringText(tokens);
                lineStrings.Add(lineString);
                nextToken = GetNextCloserOrComma(tokens);
            }
            return factory.ConstructPolylineBag(lineStrings.ToArray());
        }

        /// <summary>  
        /// Creates a <c>MultiPolygon</c> using the next token in the stream.
        /// </summary>
        /// <param name="tokens">Tokenizer over a stream of text in Well-known Text
        ///   format. The next tokens must form a MultiPolygon Text.
        /// </param>
        /// <param name="factory"> </param>
        /// <returns>
        /// A <c>MultiPolygon</c> specified by the next
        /// token in the stream, or if if the coordinates used to create the
        /// <c>Polygon</c> shells and holes do not form closed linestrings.</returns>
        private PolygonBag2<double> ReadMultiPolygonText(IEnumerator<Token> tokens)
        {
            string nextToken = GetNextEmptyOrOpener(tokens);
            if (nextToken.Equals("EMPTY"))
                return null;

            var polygons = new List<Polygon2<double>>();
            var polygon = ReadPolygonText(tokens);
            polygons.Add(polygon);
            nextToken = GetNextCloserOrComma(tokens);
            while (nextToken.Equals(","))
            {
                polygon = ReadPolygonText(tokens);
                polygons.Add(polygon);
                nextToken = GetNextCloserOrComma(tokens);
            }
            return factory.ConstructPolygonBag(polygons.ToArray());
        }

        /// <summary>
        /// Creates a <c>GeometryCollection</c> using the next token in the
        /// stream.
        /// </summary>
        /// <param name="tokens">
        ///   Tokenizer over a stream of text in Well-known Text
        ///   format. The next tokens must form a &lt;GeometryCollection Text.
        /// </param>
        /// <param name="factory"> </param>
        /// <returns>
        /// A <c>GeometryCollection</c> specified by the
        /// next token in the stream.</returns>
        //private IGeometryCollection ReadGeometryCollectionText(IEnumerator<Token> tokens, IGeometryFactory factory)
        //{
        //    string nextToken = GetNextEmptyOrOpener(tokens);
        //    if (nextToken.Equals("EMPTY"))
        //        return factory.CreateGeometryCollection(new IGeometry[] { });

        //    var geometries = new List<IGeometry>();
        //    var geometry = ReadGeometryTaggedText(tokens);
        //    geometries.Add(geometry);
        //    nextToken = GetNextCloserOrComma(tokens);
        //    while (nextToken.Equals(","))
        //    {
        //        geometry = ReadGeometryTaggedText(tokens);
        //        geometries.Add(geometry);
        //        nextToken = GetNextCloserOrComma(tokens);
        //    }
        //    return factory.CreateGeometryCollection(geometries.ToArray());
        //}
    }
}
