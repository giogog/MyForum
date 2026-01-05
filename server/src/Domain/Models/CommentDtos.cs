namespace Domain.Models;
public record CommentDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public int? ParentCommentId { get; init; }
    public string Body { get; set; }
    public string AuthorFullName { get; set; }
    public DateTime Created { get; init; }
    public string Username { get; set; }
    public CommentType Type { get; set; }
    public CommentDto()
    {

    }
}
public record CreateCommentDto(int TopicId, int? ParentCommentId, string Body);
public record UpdateCommentDto(string Body);

//public record ReplyDto
//{
//    public int Id { get; init; }
//    public int UserId { get; init; }
//    public int CommentId { get; init; }
//    public string Body { get; set; }
//    public string AuthorFullName { get; set; }
//    public string Username { get; set; }
//    public ReplyDto()
//    {

//    }
//}
//public record CreateReplyDto(int TopicId,int CommentId, string Body);
//public record UpdateReplyDto(int CommentId, string Body);