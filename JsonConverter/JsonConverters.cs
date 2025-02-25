using System.Text.Json;
using ChessLogic;
using ChessLogic.Moves;
using ChessLogic.Pieces;

#pragma warning disable CS8603 // Possible null reference return.


namespace JsonConverter
{
	public class PieceConverter : System.Text.Json.Serialization.JsonConverter
	{
		public Piece Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject)
				throw new JsonException("Start of object not found");

			using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
			{
				JsonElement root = doc.RootElement;

				PieceType type = (PieceType)root.GetProperty("type").GetInt32();
				Player color = (Player)root.GetProperty("color").GetInt32();
				bool hasMoved = root.GetProperty("hasMoved").GetBoolean();

				Piece piece;
				switch (type)
				{
					case PieceType.Pawn: piece = new Pawn(color); piece.HasMoved = hasMoved; break;
					case PieceType.Rook: piece = new Rook(color); piece.HasMoved = hasMoved; break;
					case PieceType.Bishop: piece = new Bishop(color); piece.HasMoved = hasMoved; break;
					case PieceType.Queen: piece = new Queen(color); piece.HasMoved = hasMoved; break;
					case PieceType.King: piece = new King(color); piece.HasMoved = hasMoved; break;
					case PieceType.Knight: piece = new Knight(color); piece.HasMoved = hasMoved; break;
					default:
						throw new JsonException("Unsupported piece type.");
				}
				return piece;
			}
		}

		public void Write(Utf8JsonWriter writer, Piece value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber("type", (int)value.Type);
			writer.WriteNumber("color", (int)value.Color);
			writer.WriteBoolean("hasMoved", value.HasMoved);
			writer.WriteEndObject();
		}
	}
	public class PieceArrayConverter : System.Text.Json.Serialization.JsonConverter
	{
		public override Piece[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
			{
				var array = doc.RootElement.EnumerateArray();
				int rows = array.Count();
				int cols = array.First().GetArrayLength();

				Piece[,] result = new Piece[rows, cols];

				int i = 0;
				foreach (var row in array)
				{
					int j = 0;
					foreach (var piece in row.EnumerateArray())
					{
						string RawText = piece.GetRawText();
						if (RawText == "null")
							result[i, j++] = null;
						else
							result[i, j++] = JsonSerializer.Deserialize<Piece>(piece.GetRawText(), options);
					}
					i++;
				}

				return result;
			}
		}

		public override void Write(Utf8JsonWriter writer, Piece[,] value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();

			for (int i = 0; i < value.GetLength(0); i++)
			{
				writer.WriteStartArray();

				for (int j = 0; j < value.GetLength(1); j++)
				{
					JsonSerializer.Serialize(writer, value[i, j], options);
				}

				writer.WriteEndArray();
			}

			writer.WriteEndArray();
		}
	}
	public class BoardConverter : JsonConverter<Board>
	{
		public override Board Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
			{
				JsonElement root = doc.RootElement;

				// Extract the pieces array
				JsonElement piecesElement = root.GetProperty("pieces");
				Piece[,] pieces = JsonSerializer.Deserialize<Piece[,]>(piecesElement.GetRawText(), options);

				// Extract other properties
				bool wasLastPieceMovedPawn = root.GetProperty("wasLastPieceMovedPawn").GetBoolean();
				JsonElement lastPawnMovedElement = root.GetProperty("lastPawnMoved");
				Position lastPawnMoved = JsonSerializer.Deserialize<Position>(lastPawnMovedElement.GetRawText(), options);

				bool wasLastMoveDouble = root.GetProperty("wasLastMoveDouble").GetBoolean();

				// Create and return the Board instance
				return new Board
				{
					pieces = pieces,
					WasLastPieceMovedPawn = wasLastPieceMovedPawn,
					LastPawnMoved = lastPawnMoved,
					WasLastMoveDouble = wasLastMoveDouble
				};
			}
		}

		public override void Write(Utf8JsonWriter writer, Board value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();

			// Serialize the pieces array
			writer.WriteStartObject("pieces");
			JsonSerializer.Serialize(writer, value.pieces, options);
			writer.WriteEndObject();

			// Serialize other properties
			writer.WriteBoolean("wasLastPieceMovedPawn", value.WasLastPieceMovedPawn);
			writer.WriteStartObject("lastPawnMoved");
			JsonSerializer.Serialize(writer, value.LastPawnMoved, options);
			writer.WriteEndObject();

			writer.WriteBoolean("wasLastMoveDouble", value.WasLastMoveDouble);

			writer.WriteEndObject();
		}
	}

	public class PositionConverter : JsonConverter<Position>
	{
		public override Position Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
			{
				JsonElement root = doc.RootElement;
				return new Position
				(
					root.GetProperty("row").GetInt32(),
					root.GetProperty("column").GetInt32()
				);
			}
		}

		public override void Write(Utf8JsonWriter writer, Position value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber("row", value.Row);
			writer.WriteNumber("column", value.Column);
			writer.WriteEndObject();
		}
	}

	public class MoveConverter : JsonConverter<Move>
	{
		public override Move Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
			{
				JsonElement root = doc.RootElement;

				MoveType Type = (MoveType)root.GetProperty("type").GetInt32();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
				Position FromPos = JsonSerializer.Deserialize<Position>(root.GetProperty("fromPos").GetRawText(), options);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
				Position ToPos = JsonSerializer.Deserialize<Position>(root.GetProperty("toPos").GetRawText(), options);
				Move move;
				switch (Type)
				{
					case MoveType.Normal: move = new NormalMove(FromPos, ToPos); break;
					case MoveType.EnPassant: move = new EnPassant(FromPos, ToPos); break;
					case MoveType.PawnPromotion: move = new PawnPromotion(FromPos, ToPos); break;
					case MoveType.CastleKS: move = new CastleKs(FromPos, ToPos); break;
					case MoveType.CastleQS: move = new CastleQs(FromPos, ToPos); break;
					default:
						throw new JsonException("Unsupported piece type.");
				}
				return move;
			}
		}

		public override void Write(Utf8JsonWriter writer, Move value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber("type", (int)value.Type);
			writer.WritePropertyName("fromPos");
			new PositionConverter().Write(writer, value.FromPos, options);
			writer.WritePropertyName("toPos");
			new PositionConverter().Write(writer, value.ToPos, options);
			writer.WriteEndObject();
		}
	}


}