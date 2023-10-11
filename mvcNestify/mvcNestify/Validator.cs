using System.ComponentModel.DataAnnotations;

namespace mvcNestify
{
    public static class ValidationHelper
    {
        //helper for DOB validation
        public static int GetAge(DateTime dob)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - dob.Year;
            if (dob > today.AddYears(-age))
                age--;

            return age;
        }
    }

}
