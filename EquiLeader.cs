using System;
using System.Linq;

class Solution {
    public void splitArray(int[] array, int index, out int[] first, out int[] second ) {
        first = array.Take(index).ToArray();
        second = array.Skip(index).ToArray();
    }
    public int getDominator(int[] array) {
        //Set integers
        int length = array.Length;
        int size = 0;
        int count = 0;
        int candidate = -1;
        int value = -1;
        int leader = -1;

        //Check for most frequent
        for (int i = 0; i < length; i++) {
            if (size == 0) {
                size += 1;
                value = array[i];
            } else {
                if (value != array[i]) {
                    size -= 1;
                } else {
                    size += 1;
                }
            }
        }
        //Check if value occured most frequently
        if (size > 0) {
            candidate = value;
        }
        for (int i = 0; i < length; i++) {
            if (array[i] == candidate) {
                count += 1;
            }
        }
        if (count > length / 2) {
            leader = candidate;
        }
        return leader;
    }
    public int solution(int[] A) {
        int size = A.Length;
        int count = 0;
        //Loop through all indices
        for (int i = 1; i <= size; i++) {
            int[] left;
            int[] right;
            //Split array on indice
            splitArray(A, i, out left, out right);

            //Get leader of each half
            int leftLeader = getDominator(left);
            int rightLeader = getDominator(right);

            //Compare
            if (leftLeader == rightLeader && leftLeader != -1) {
                count++;
            }
        }
        return count;
    }
}
