
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PPS.API.Common.Helpers
{
	public static class Json
	{
		public static JsonSerializerSettings Settings
		{
			get
			{

				return new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore,
					//DateParseHandling = Newtonsoft.Json.DateParseHandling.DateTimeOffset,
					//DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
					//DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat,
					//ContractResolver = new CamelCasePropertyNamesContractResolver(), 
					//Converters = { new DateSerializerConverter() }
					//Converters = { new IsoDateTimeConverter(){ DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal & System.Globalization.DateTimeStyles.AdjustToUniversal } }
					// Converters = { new IsoDateTimeConverter() }
					Converters = 
					{	
						new IsoDateTimeConverter() {}, 
						//new JsonEnumConverter(),
						// new DateTimeConverter() { DateTimeStyles = System.Globalization.DateTimeStyles.AdjustToUniversal}, 
						new JsonDecimalConverter(),
						new JsonDoubleConverter(),
                        new JsonEnumConverter(),
						//new JsonDecimalConverter(),
						//new JsonEmptyListConverter()
						//new JsonArrayConverter()
					}
				};
			}
		}

		public static string Encode(dynamic target)
		{
			if (target == null) return null;

			return JsonConvert.SerializeObject(target, Formatting.None, Json.Settings);
		}

		public static object Decode(string target, Type type)
		{

			//create a simplified JSON structure (no need for reflection then)
			return JsonConvert.DeserializeObject(target, type, Json.Settings);
		}

		public static dynamic Decode(string target)
		{
			//create a simplified JSON structure (no need for reflection then)
			return JsonConvert.DeserializeObject(target, Json.Settings);
		}
	}

	public class JsonEnumConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType.IsEnum || (IsNullable(objectType) && Nullable.GetUnderlyingType(objectType).IsEnum);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return Enum.Parse(IsNullable(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType, reader.Value.ToString());
		}

		private bool IsNullable(Type type)
		{
			return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
		}
	}

	public class JsonDecimalConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Decimal) || objectType == typeof(Decimal?);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(string.Format("{0:G29}", decimal.Parse(value.ToString())));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return string.IsNullOrEmpty(reader.Value.ToString()) ? null : (Decimal?)Decimal.Parse(reader.Value.ToString());
		}
	}

	public class JsonDoubleConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(double) || objectType == typeof(double?);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(double.Parse(value.ToString()));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return double.Parse(reader.Value.ToString());
		}
	}

	//public class JsonArrayConverter : JsonConverter
	//{
	//    public override bool CanConvert(Type objectType)
	//    {
	//        return objectType.IsArray && objectType != typeof(byte[]);
	//    }

	//    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	//    {
	//        writer.WriteValue(value);
	//    }

	//    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	//    {
	//        return Json.Decode(reader.Value.ToString(), objectType);
	//    }
	//}

	//public static class stringextensions
	//{
	//    public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
	//    {
	//        return string.Format(provider, format, args);
	//    }
	//}

//    public class DateTimeConverter : DateTimeConverterBase
//    {
//        public Type GetObjectType(object v)
//        {
//            return (v != null) ? v.GetType() : null;
//        }

//        public bool IsNullableType(Type t)
//        {
//            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
//        }

		

//        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

//        private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
//        private string _dateTimeFormat;
//        private CultureInfo _culture;

//        /// <summary>
//        /// Gets or sets the date time styles used when converting a date to and from JSON.
//        /// </summary>
//        /// <value>The date time styles used when converting a date to and from JSON.</value>
//        public DateTimeStyles DateTimeStyles
//        {
//            get { return _dateTimeStyles; }
//            set { _dateTimeStyles = value; }
//        }

//        /// <summary>
//        /// Gets or sets the date time format used when converting a date to and from JSON.
//        /// </summary>
//        /// <value>The date time format used when converting a date to and from JSON.</value>
//        public string DateTimeFormat
//        {
//            get { return _dateTimeFormat ?? string.Empty; }
//            set { _dateTimeFormat = (string.IsNullOrEmpty(value)) ? null : value; }
//        }

//        /// <summary>
//        /// Gets or sets the culture used when converting a date to and from JSON.
//        /// </summary>
//        /// <value>The culture used when converting a date to and from JSON.</value>
//        public CultureInfo Culture
//        {
//            get { return _culture ?? CultureInfo.CurrentCulture; }
//            set { _culture = value; }
//        }

//        /// <summary>
//        /// Writes the JSON representation of the object.
//        /// </summary>
//        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
//        /// <param name="value">The value.</param>
//        /// <param name="serializer">The calling serializer.</param>
//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//        {
//            string text;

//            if (value is DateTime)
//            {
//                DateTime dateTime = (DateTime)value;

//                if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
//                  || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
//                    dateTime = dateTime.ToUniversalTime();

//                text = dateTime.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);
//            }
//#if !PocketPC && !NET20
//            else if (value is DateTimeOffset)
//            {
//                DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
//                if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
//                  || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
//                    dateTimeOffset = dateTimeOffset.ToUniversalTime();

//                text = dateTimeOffset.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);
//            }
//#endif
//            else
//            {
//                throw new Exception("Unexpected value when converting date. Expected DateTime or DateTimeOffset, got {0}.".FormatWith(CultureInfo.InvariantCulture, GetObjectType(value)));
//            }

//            writer.WriteValue(text);
//        }

//        /// <summary>
//        /// Reads the JSON representation of the object.
//        /// </summary>
//        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
//        /// <param name="objectType">Type of the object.</param>
//        /// <param name="existingValue">The existing value of object being read.</param>
//        /// <param name="serializer">The calling serializer.</param>
//        /// <returns>The object value.</returns>
//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//        {
//            bool nullable = IsNullableType(objectType);
//            Type t = (nullable)
//              ? Nullable.GetUnderlyingType(objectType)
//              : objectType;

//            if (reader.TokenType == JsonToken.Null)
//            {
//                if (!IsNullableType(objectType))
//                    throw new Exception("Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));

//                return null;
//            }

//            if (reader.TokenType == JsonToken.Date)
//            {
//#if !PocketPC && !NET20
//                if (t == typeof(DateTimeOffset))
//                    return new DateTimeOffset((DateTime)reader.Value);
//#endif

//                return reader.Value;
//            }

//            if (reader.TokenType != JsonToken.String)
//                throw new Exception("Unexpected token parsing date. Expected String, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));

//            string dateText = reader.Value.ToString();

//            if (string.IsNullOrEmpty(dateText) && nullable)
//                return null;

//#if !PocketPC && !NET20
//            if (t == typeof(DateTimeOffset))
//            {
//                if (!string.IsNullOrEmpty(_dateTimeFormat))
//                    return DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
//                else
//                    return DateTimeOffset.Parse(dateText, Culture, _dateTimeStyles);
//            }
//#endif

//            if (!string.IsNullOrEmpty(_dateTimeFormat))
//                return DateTime.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
//            else
//                return DateTime.Parse(dateText, Culture, _dateTimeStyles);
//        }
//    }
}
