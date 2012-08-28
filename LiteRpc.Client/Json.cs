namespace DynamicJson
{
	using System;
	using System.Collections.Generic;
	using System.Dynamic;
	using System.Text;
	using System.Text.RegularExpressions;

	public static class Json
	{
		public static dynamic CreateObject(string json)
		{
			var typeHash = json.GetHashCode();
			var re = new Regex(@"\[?{?(.*)}?\]?");
			Match match = re.Match(json);
			if (!match.Success)
			{
				throw new ArgumentException("Invalid JSON string");
			}

			var typeBuilderStack = new Stack<DynamicTypeBuilder>();
			var nameStack = new Stack<string>();
			var valueStack = new Stack<Dictionary<string, object>>();
			var currentValues = new Dictionary<string, object>();
			var currentArray = new List<object>();

			DynamicTypeBuilder currentTypeBuilder = null;
			string name = null;
			var nameBuilder = new StringBuilder();
			var valueBuilder = new StringBuilder();
			bool isValue = false;
			bool isArray = false;
			object result = null;

			foreach (var chr in json)
			{
				switch (chr)
				{
					case '[':
						isArray |= true;
						break;
					case ']':
						isArray &= false;
						if (currentTypeBuilder != null)
						{
							currentTypeBuilder.AddProperty(name, typeof(object[]));
						}
						else
						{
							result = currentArray.ToArray();
						}
						break;
					case '{':
						if (isValue)
						{
							nameStack.Push(name);
							typeBuilderStack.Push(currentTypeBuilder);
							valueStack.Push(currentValues);
							currentValues = new Dictionary<string, object>();
							currentTypeBuilder = new DynamicTypeBuilder(string.Concat("anonym_", typeHash, '_', typeBuilderStack.Count));
							isValue &= false;
						}
						else
						{
							currentTypeBuilder = new DynamicTypeBuilder(string.Concat("anonym_", typeHash));
						}
						break;
					case '}':
						if (isValue)
						{
							var value = Json.ParseItem(valueBuilder.ToString());
							if (value != null)
							{
								currentTypeBuilder.AddProperty(name, value.GetType());
							}
							else
							{
								currentTypeBuilder.AddProperty(name, typeof(object));
							}
							currentValues[name] = value;
							valueBuilder.Clear();
							isValue &= false;
						}
						var type = currentTypeBuilder.CreateType();
						var instance = Activator.CreateInstance(type);
						foreach (var item in currentValues)
						{
							type.GetProperty(item.Key).SetValue(instance, item.Value, null);
						}
						if (typeBuilderStack.Count > 0)
						{
							currentValues = valueStack.Pop();
							name = nameStack.Pop();
							currentValues[name] = instance;
							currentTypeBuilder = typeBuilderStack.Pop();
							currentTypeBuilder.AddProperty(name, type);
						}
						else
						{
							result = instance;
						}
						break;
					case ':':
						name = nameBuilder.ToString();
						nameBuilder.Clear();
						isValue |= true;
						break;
					case ',':
						if (isArray)
						{
							if (currentTypeBuilder == null)
							{
								var value = Json.ParseItem(valueBuilder.ToString());
								currentArray.Add(value);
								valueBuilder.Clear();
							}
							else
							{

							}
						}
						else if (isValue)
						{
							var value = Json.ParseItem(valueBuilder.ToString());
							if (value != null)
							{
								currentTypeBuilder.AddProperty(name, value.GetType());
							}
							else
							{
								currentTypeBuilder.AddProperty(name, typeof(object));
							}
							currentValues[name] = value;
							valueBuilder.Clear();
							isValue &= false;
						}
						break;
					case ' ':
					case '\t':
					case '\n':
						if (nameBuilder.Length > 0)
						{
							throw new ArgumentException("Property names can contain only alphanumeric charcaters.");
						}
						break;
					case '\'':
					case '"':
						if (isValue)
						{
							valueBuilder.Append(chr);
						}
						break;
					default:
						if (isArray || isValue)
						{
							valueBuilder.Append(chr);
						}
						else
						{
							if (char.IsLetter(chr))
							{
								nameBuilder.Append(chr);
							}
							else if (char.IsLetterOrDigit(chr))
							{
								if (nameBuilder.Length > 0)
								{
									nameBuilder.Append(chr);
								}
								else
								{
									throw new ArgumentException("Property names must start with letter.");
								}
							}
							else
							{
								throw new ArgumentException("Property names can contain only alphanumeric charcaters.");
							}
						}
						break;
				}
			}

			return result;
		}

		private static object ParseItem(string value)
		{
			var re = new Regex(@"['""](.*)['""]");
			Match match = re.Match(value);
			if (match.Success)
			{
				return match.Groups[1].Value;
			}
			else
			{
				if (value.Contains("."))
				{
					{
						decimal result;
						if (decimal.TryParse(value, out result))
						{
							return result;
						}
					}
					{
						double result;
						if (double.TryParse(value, out result))
						{
							return result;
						}
					}
				}
				else
				{
					{
						int result;
						if (int.TryParse(value, out result))
						{
							return result;
						}
					}
					{
						long result;
						if (long.TryParse(value, out result))
						{
							return result;
						}
					}
				}
				{
					decimal result;
					if (decimal.TryParse(value, out result))
					{
						return result;
					}
				}
				{
					DateTime result;
					if (DateTime.TryParse(value, out result))
					{
						return result;
					}
				}
				return null;
			}
		}

	}
}
