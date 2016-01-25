#include <cmath>
using namespace std;

namespace Exercise
{

	public ref class SortSummary
	{
	private:
			System::UInt32 _comparisons;
			System::UInt32 _swaps;

	public:
		property System::UInt32  Comparisons
		{
			System::UInt32  get()
			{
				return _comparisons;
			}
		private:
			void set(System::UInt32  value)
			{
				_comparisons = value;
			}

		}
		property System::UInt32  Swaps
		{
			System::UInt32  get()
			{
				return _swaps;
			}
		private:
			void set(System::UInt32  value)
			{
				_swaps = value;
			}
		}

		SortSummary(System::UInt32 comparisons, System::UInt32 swaps)
		{
			_comparisons = comparisons;
			_swaps = swaps;
		}
	};
}