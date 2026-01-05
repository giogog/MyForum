using Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Forum
{
    public int Id { get; set; }

    [Required]
    [StringLength(20, ErrorMessage = "Forum must be simplier.")]
    public string Title { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;

    public int TopicNum => Topics?.Count ?? 0;

    [EnumDataType(typeof(State))]
    public State State { get; set; } = State.Pending;

    [EnumDataType(typeof(Status))]
    public Status Status { get; set; } = Status.Active;

    public ICollection<Topic>? Topics { get; set; }
    [Required]
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }


}
