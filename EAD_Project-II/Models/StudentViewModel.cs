using System;
using System.ComponentModel.DataAnnotations;

namespace EAD_Project_II 
{
	public class StudentViewModel
	{
		public int Id { get; set; }

		[Display(Name = "Full Name")]
		[Required(ErrorMessage = "Please enter the full name")]
		[StringLength(100)]
		public string? FullName { get; set; }

		[Required(ErrorMessage = "Please enter roll number")]
		[Display(Name = "Roll Number")]
		[StringLength(20)]
		public string? RollNumber { get; set; }

        [Required(ErrorMessage = "Enter your email")]
        [Display(Name = "Email Address")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address")]
		[StringLength(100)]
		public string? EmailAddress { get; set; }

        [Required(ErrorMessage = "Please select gender")]
        [StringLength(20)]
		public string? Gender { get; set; }

        [Required]
        [Display(Name = "Interest")]
		[StringLength(100)]
		public string? Interest { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
		[DataType(DataType.Date)]
		public DateTime? Dob { get; set; }

        [Required]
        [StringLength(100)]
		public string? Subject { get; set; }

        [Required]
        [StringLength(50)]
		public string? City { get; set; }

        [Required(ErrorMessage = "Please select department")]
        [StringLength(50)]
		public string? Dept { get; set; }

        [Required(ErrorMessage = "Please select degree")]
        [StringLength(50)]
		public string? Degree { get; set; }

        
        [Display(Name = "Start Date")]
		[DataType(DataType.Date)]
		public DateTime? StartDate { get; set; }

       
        [Display(Name = "End Date")]
		[DataType(DataType.Date)]
		public DateTime? EndDate { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }

        public List<string> ExistingInterests { get; set; }

        [Display(Name = "Selected Interest")]
        [StringLength(100)]
        public string? SelectedInterest { get; set; }

        public StudentViewModel()
        {
            ExistingInterests = new List<string>();
        }

    }
}
