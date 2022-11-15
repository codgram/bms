namespace Application.Model;

public class Subscription : Detail
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

    public int Companies { get; set; }
    public int Members { get; set; }
    public int Menus { get; set; }
    public int Items { get; set; }

}