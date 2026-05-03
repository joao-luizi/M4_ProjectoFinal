using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLibrary.Models;

public class UserPayment
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string UserId { get; set; }
    public int PackageId { get; set; }
    public Package PackageBought { get; set; }
    public DateOnly BuyDate { get; set; }
    public DateOnly DueDate { get; set; }
    public int? AmountOfClasses { get; set; }

}
