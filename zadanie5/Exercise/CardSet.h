#include <cmath>
#include <time.h>
#include "Card.h"
#include "SortSummary.h"
#include "SortAlgorithm.h"
using namespace std;
using namespace System::Collections::Generic;

namespace Exercise
{
	generic<class CARD> where CARD: Card, System::IComparable<CARD>, gcnew()
		public ref class CardSet : IEnumerable<CARD>, System::ICloneable, System::IComparable<CardSet<CARD>^>
	{
	private:
		array<CARD>^ _cards;
		property array<CARD>^ Cards
		{
			array<CARD>^ get()
			{
				return _cards;
			}
		private:
			void set(array<CARD>^ value)
			{
				_cards = value;
			}
		}
		short comparisons;
		short swaps;

		void Reset()
		{
			comparisons = 0;
			swaps = 0;
		}

		void QuickSort(int left, int right)
		{

			int i = left, j = right;
			CARD pivot = Cards[(left + right) / 2];

			while (i <= j)
			{
				while (Cards[i]->CompareTo(pivot) < 0)
				{
					i++;
					comparisons++;
				}

				while (Cards[j]->CompareTo(pivot) > 0)
				{
					j--;
					comparisons++;
				}

				if (i <= j)
				{
					SwapCards(i, j);

					i++;
					j--;
				}
			}

			// Recursive calls
			if (left < j)
			{
				QuickSort(left, j);
			}

			if (i < right)
			{
				QuickSort(i, right);
			}
		}


		void BubbleSort()
		{

			for (int write = 0; write < Cards->Length; write++)
			{
				for (int sort = 0; sort < Cards->Length - 1; sort++)
				{
					if (CompareCards(sort, sort + 1) > 0)
					{
						SwapCards(sort, sort + 1);
					}
				}
			}
		}

		void SwapCards(int index1, int index2)
		{
			CARD temp = Cards[index1];
			Cards[index1] = Cards[index2];
			Cards[index2] = temp;
			swaps++;
		}

		int CompareCards(int index1, int index2)
		{
			comparisons++;
			return Cards[index1]->CompareTo(Cards[index2]);
		}

	public:


		CardSet(array<CARD>^ cards)
		{
			_cards = (array<CARD>^)cards->Clone();
		}
		~CardSet()
		{
		}
		virtual System::Collections::IEnumerator^ GetEnumeratorNonGeneric() = System::Collections::IEnumerable::GetEnumerator
		{
			return GetEnumerator();
		}

		virtual IEnumerator<CARD>^ GetEnumerator()
		{
			return safe_cast<IEnumerable<CARD>^>(_cards)->GetEnumerator();
		}
		virtual Object^ Clone()
		{
			int len = Cards->Length;
			array<CARD> ^newCards = gcnew array<CARD>(Cards->Length);
			for (int i = 0; i < len; i++)
				newCards[i] = safe_cast<CARD>(Cards[i]->Clone());
			return  safe_cast<Object^>(gcnew CardSet(newCards));
		}
		

		virtual int CompareTo(CardSet<CARD>^ other)
		{
			return 1;
		}

		property CARD default[int]
		{
			CARD get(int index)
			{
				return _cards[index];
		}
		void set(int index, CARD value)
		{
			_cards[index] = value;
		}
		}

			void Randomize()
		{

			int n = Cards->Length;
			while (n > 1) {
				n--;
				System::DateTime ^now = System::DateTime::Now;
				long second = now->Millisecond;
				int k = (second % n);
				SwapCards(k, n);
			}

		}
		Exercise::SortSummary^ Sort(SortAlgorithm algorithm)
		{
			Reset();
			if (algorithm.Equals(SortAlgorithm::Quick))
			{
				QuickSort(0, Cards->Length - 1);
			}
			else if (algorithm.Equals(SortAlgorithm::Bubble))
			{
				BubbleSort();
			}
			return gcnew SortSummary(comparisons, swaps);
		}
	};
}

