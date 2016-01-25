#include <cmath>
#include "CardColor.h"
#include "CardType.h"
#include <time.h>
#include <math.h>
using namespace std;

namespace Exercise
{
	public ref class  Card : System::IComparable<Card^>, System::ICloneable
	{
	protected:
		CardColor _color;
		CardType _type;
	public:
		property CardColor Color
		{
			CardColor get()
			{
				return _color;
			}
		private:
			void set(CardColor value)
			{
				_color = value;
			}

		}
		property CardType Type
		{
			CardType get()
			{
				return _type;
			}
		private:
			void set(CardType value)
			{
				_type = value;
			}
		}
	
		Card()
		{
		}

		Card(CardType type,CardColor color)
		{
			Color = color;
			Type = type;
		}

		~Card()
		{
			
		}

		virtual int CompareTo(Card^ other)
		{
			return GetPowerOfTwo((short)Type) - GetPowerOfTwo((short)other->Type);
		}

		virtual Object^ Clone()
		{
			return MemberwiseClone();
			
		}

		short GetPowerOfTwo(short number)
		{
			short power = 0;
			if (number&(number - 1)) return 0;
			short temp = number;
			while(temp > 1)
			{
				temp /= 2;
				power++;
			}
			return power;
		}

		virtual bool Equals(Object^ o) override

		{
			Card^	obj = (Card^)o;
			if (this == nullptr && obj == nullptr)
            {
                return true;
            }
            else
            {
                if ((this == nullptr) || (this == nullptr))
                {
                    return false;
                }
                else
                {
					return Color.Equals(obj->Color) && Type.Equals(obj->Type);
                }
            }
					
		}
		virtual int GetHashCode() override
		{
			return (int)Color * (int)Type;
		}
	
	};
}