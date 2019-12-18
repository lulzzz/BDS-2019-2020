using System;

namespace StreamProcessing.Function
{
    public static class NewEvent
    {
        public static MyType CreateNewEvent(MyType e, string Key, string Value)
        {
            //Console.WriteLine($"e.key = {e.key}, e.value = {e.value}, Key = {Key}, Value = {Value}");
            string[] key_parts = Key.Split(" ");
            string[] value_parts = Value.Split(" ");

            string[] e_Keys = e.key.Split(" ");
            string[] e_Values = e.value.Split(" ");
            int len_key = 0;
            int len_value = 0;
            if (e.key.Length > 0) len_key = e_Keys.Length;
            if (e.value.Length > 0) len_value = e_Values.Length;

            string new_key = GetCols(key_parts, len_key, len_value, e_Keys, e_Values);
            string new_value = GetCols(value_parts, len_key, len_value, e_Keys, e_Values);

            return new MyType(new_key, new_value, e.timestamp);
        }

        public static string GetCols(string[] parts, int len_key, int len_value, string[] e_Keys, string[] e_Values)
        {
            string new_col = "";

            foreach (var item in parts)
            {
                if (item.Length == 0) continue;
                int index = Convert.ToInt16(item);
                if (index < len_key) new_col += e_Keys[index] + " ";
                else if (index < len_key + len_value) new_col += e_Values[index - len_key] + " ";
                else throw new Exception($"Exception: cannot create new event, because index is out of bound. len_key = {len_key}, len_value = {len_value}, index = {index}");
            }
            return new_col;
        }
    }
}
