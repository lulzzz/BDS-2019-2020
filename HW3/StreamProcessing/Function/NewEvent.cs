using System;

namespace StreamProcessing.Function
{
    public static class NewEvent
    {
        public static MyType CreateNewEvent(MyType e, string Key, string Value)
        {
            string[] key_parts = Key.Split(" ");
            string[] value_parts = Value.Split(" ");

            string[] e_Keys = e.key.Split(" ");
            string[] e_Values = e.value.Split(" ");
            int len_key = e_Keys.Length;
            int len_value = e_Values.Length;

            string new_key = getCols(key_parts, len_key, len_value, e_Keys, e_Values);
            string new_value = getCols(value_parts, len_key, len_value, e_Keys, e_Values);

            return new MyType(new_key, new_value, e.timestamp);
        }

        public static string getCols(string[] parts, int len_key, int len_value, string[] e_Keys, string[] e_Values)
        {
            string new_col = "";

            foreach (var item in parts)
            {
                int index = Convert.ToInt16(item);
                if (index < len_key) new_col += e_Keys[index] + " ";
                else if (index < len_key + len_value) new_col += e_Values[index - len_key] + " ";
                else throw new Exception($"Exception: cannot create new event, because index is out of bound. ");
            }
            return new_col;
        }
    }
}
