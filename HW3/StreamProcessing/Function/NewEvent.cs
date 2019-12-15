using System;

namespace StreamProcessing.Function
{
    public static class NewEvent
    {
        public static MyType CreateNewEvent(MyType e, String Key, String Value)
        {
            String[] key_parts = Key.Split(" ");
            String[] value_parts = Value.Split(" ");

            String[] e_Keys = e.key.Split(" ");
            String[] e_Values = e.value.Split(" ");
            int len_key = e_Keys.Length;
            int len_value = e_Values.Length;

            String new_key = getCols(key_parts, len_key, len_value, e_Keys, e_Values);
            String new_value = getCols(value_parts, len_key, len_value, e_Keys, e_Values);

            return new MyType(new_key, new_value, e.timestamp);
        }

        public static MyType CreateNewEvent(MyType e1, MyType e2, String Key, String Value)
        {
            String[] key_parts = Key.Split(",");
            String key_1 = key_parts[0];
            String key_2 = key_parts[1];

            String[] value_parts = Value.Split(",");
            String value_1 = value_parts[0];
            String value_2 = value_parts[1];

            MyType new_e1 = CreateNewEvent(e1, key_1, value_1);
            MyType new_e2 = CreateNewEvent(e2, key_2, value_2);

            return new MyType(new_e1.key + " " + new_e2.key, new_e1.value + " " + new_e2.value, e1.timestamp);
        }

        public static String getCols(String[] parts, int len_key, int len_value, String[] e_Keys, String[] e_Values)
        {
            String new_col = "";

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
