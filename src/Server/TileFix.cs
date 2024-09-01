using Terraria;
using Terraria.ID;

namespace Ruby.Server;

internal class TileFix : WorldGen
{
    internal static void Initialize()
    {
		On.Terraria.IO.WorldFile.SaveWorldTiles += SaveWorldTiles;

	}

    private static int SaveWorldTiles(On.Terraria.IO.WorldFile.orig_SaveWorldTiles orig, object writer)
    {
		try
		{
			return SaveWorldTiles((BinaryWriter)writer);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}

		return (int)((BinaryWriter)writer).BaseStream.Position;
    }

    static unsafe int SaveWorldTiles(BinaryWriter writer)
	{
		byte[] array = new byte[16];
		for (int i = 0; i < Main.maxTilesX; i++)
		{
			float num = (float)i / (float)Main.maxTilesX;
			Main.statusText = Lang.gen[49].Value + " " + (int)(num * 100f + 1f) + "%";
			int num2;
			for (num2 = 0; num2 < Main.maxTilesY; num2++)
			{
				TileData* tile = Main.tile[i, num2].ptr;
				int position = 4;
				byte b2;
				byte b;
				byte b3;
				byte b4 = (b3 = (b2 = (b = 0)));
				bool flag = false;
				if (tile->active())
				{
					flag = true;
				}
				if (flag)
				{
					b4 = (byte)(b4 | 2u);
					array[position] = (byte)tile->type;
					position++;
					if (tile->type > 255)
					{
						array[position] = (byte)(tile->type >> 8);
						position++;
						b4 = (byte)(b4 | 0x20u);
					}
					if (Main.tileFrameImportant[tile->type])
					{
						array[position] = (byte)((uint)tile->frameX & 0xFFu);
						position++;
						array[position] = (byte)((tile->frameX & 0xFF00) >> 8);
						position++;
						array[position] = (byte)((uint)tile->frameY & 0xFFu);
						position++;
						array[position] = (byte)((tile->frameY & 0xFF00) >> 8);
						position++;
					}
					if (tile->color() != 0)
					{
						b2 = (byte)(b2 | 8u);
						array[position] = tile->color();
						position++;
					}
				}
				if (tile->wall != 0)
				{
					b4 = (byte)(b4 | 4u);
					array[position] = (byte)tile->wall;
					position++;
					if (tile->wallColor() != 0)
					{
						b2 = (byte)(b2 | 0x10u);
						array[position] = tile->wallColor();
						position++;
					}
				}
				if (tile->liquid != 0)
				{
					if (!tile->shimmer())
					{
						b4 = (tile->lava() ? ((byte)(b4 | 0x10u)) : ((!tile->honey()) ? ((byte)(b4 | 8u)) : ((byte)(b4 | 0x18u))));
					}
					else
					{
						b2 = (byte)(b2 | 0x80u);
						b4 = (byte)(b4 | 8u);
					}
					array[position] = tile->liquid;
					position++;
				}
				if (tile->wire())
				{
					b3 = (byte)(b3 | 2u);
				}
				if (tile->wire2())
				{
					b3 = (byte)(b3 | 4u);
				}
				if (tile->wire3())
				{
					b3 = (byte)(b3 | 8u);
				}
				int num4 = (tile->halfBrick() ? 16 : ((tile->slope() != 0) ? (tile->slope() + 1 << 4) : 0));
				b3 = (byte)(b3 | (byte)num4);
				if (tile->actuator())
				{
					b2 = (byte)(b2 | 2u);
				}
				if (tile->inActive())
				{
					b2 = (byte)(b2 | 4u);
				}
				if (tile->wire4())
				{
					b2 = (byte)(b2 | 0x20u);
				}
				if (tile->wall > 255)
				{
					array[position] = (byte)(tile->wall >> 8);
					position++;
					b2 = (byte)(b2 | 0x40u);
				}
				if (tile->invisibleBlock())
				{
					b = (byte)(b | 2u);
				}
				if (tile->invisibleWall())
				{
					b = (byte)(b | 4u);
				}
				if (tile->fullbrightBlock())
				{
					b = (byte)(b | 8u);
				}
				if (tile->fullbrightWall())
				{
					b = (byte)(b | 0x10u);
				}
				int num5 = 3;
				if (b != 0)
				{
					b2 = (byte)(b2 | 1u);
					array[num5] = b;
					num5--;
				}
				if (b2 != 0)
				{
					b3 = (byte)(b3 | 1u);
					array[num5] = b2;
					num5--;
				}
				if (b3 != 0)
				{
					b4 = (byte)(b4 | 1u);
					array[num5] = b3;
					num5--;
				}
				short num6 = 0;
				int num7 = num2 + 1;
				int num8 = Main.maxTilesY - num2 - 1;
				/*
				while (num8 > 0 && tile->isTheSameAs(Main.tile[i, num7]) && TileID.Sets.AllowsSaveCompressionBatching[tile->type])
				{
					num6 = (short)(num6 + 1);
					num8--;
					num7++;
				}
				*/
				num2 += num6;
				if (num6 > 0)
				{
					array[position] = (byte)((uint)num6 & 0xFFu);
					position++;
					if (num6 > 255)
					{
						b4 = (byte)(b4 | 0x80u);
						array[position] = (byte)((num6 & 0xFF00) >> 8);
						position++;
					}
					else
					{
						b4 = (byte)(b4 | 0x40u);
					}
				}
				array[num5] = b4;
				writer.Write(array, num5, position - num5);
			}
		}
		return (int)writer.BaseStream.Position;
	}
}