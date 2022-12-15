using System.ComponentModel.DataAnnotations;

namespace BlazorAppDemo01.Data;

public class Dog
{
    public int DogId { get; set; }
    [Required] public string Name { get; set; }
    [Required] public string Race { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Age cannot be negative")]
    public int Age { get; set; }
}