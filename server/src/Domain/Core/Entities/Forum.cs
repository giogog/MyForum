using Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Forum
{
    public int Id { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Forum title must be between 3 and 200 characters.")]
    public string Title { get; set; } = null!;

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public int TopicNum => Topics?.Count ?? 0;

    [EnumDataType(typeof(State))]
    public State State { get; set; } = State.Pending;

    [EnumDataType(typeof(Status))]
    public Status Status { get; set; } = Status.Active;

    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    [Required]
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;


}
