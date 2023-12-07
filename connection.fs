namespace bomLite
open System
open FSharp.Data
open FSharp.Data.Sql
module sekritz = 
  let [<Literal>] cs = "Data Source=willpc\sqlexpress;Database=M2MDATA01;Integrated Security=SSPI;trusted_connection=yes;TrustServerCertificate=True;"
open sekritz
type sql = SqlDataProvider<ConnectionString=cs, DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER_DYNAMIC, ResolutionPath="./SqlProvider/">
module connection =

  let tryGetContext () = 
    try 
      match sql.GetDataContext() with
      | ctx -> Result.Ok ctx 
    with
      | e -> Result.Error e




  let getItemMasters ()=
    let resultCtx = tryGetContext()
    match resultCtx with
      | Error e -> 
        failwith e.Message 
      | Ok c -> 
        query {
          for x in c.Dbo.ItemMasters ->
            new ItemMaster(
             x.PartNumber,
             x.Description,
             x.UnitOfMeasure,
             x.UnitPrice,
             x.RevisionNumber,
             x.IdentityColumn 
            ) 

        }
      |> List.ofSeq


  let getJobMasters ()=
    let resultCtx = tryGetContext()
    match resultCtx with
      | Result.Error e -> 
        failwith e.Message 
      | Result.Ok c -> 
        query {
          for x in c.Dbo.JobOrderMasters ->
              new JobOrderMaster( x.JobNumber, x.Description, x.SalesOrder, x.ParentJobNumber, x.HasSubJobs, x.CompanyName, x.OrderDate, x.DueDate, x.Status,x.IdentityColumn)
        }
      |> List.ofSeq


  let createJobOrderMaster (
    jobNumber: string,
    description: string,
    salesOrder: string,
    parentJobNumber: string,
    hasSubJobs: bool,
    companyName: string,
    orderDate: DateTime,
    dueDate: DateTime,
    status: string
    ) =
    let ctx = tryGetContext()
    match ctx with
      | Result.Error e -> 
        failwith e.Message 
      | Result.Ok c -> 
        let j = c.Dbo.JobOrderMasters.Create()
        j.JobNumber <- jobNumber
        j.Description <- description
        j.SalesOrder <- salesOrder
        j.ParentJobNumber <- parentJobNumber
        j.HasSubJobs <- hasSubJobs
        j.CompanyName <- companyName
        j.OrderDate <- orderDate
        j.DueDate <- dueDate
        j.Status <- status
        // j.Save()
        c.SubmitUpdates()

  let getJobOrderRows (jobNumber: string)=

    let resultCtx = tryGetContext()
    match resultCtx with
      | Result.Error e -> 
        failwith e.Message 
      | Result.Ok c -> 
        let job = 
          query {
            for x in c.Dbo.JobOrderMasters do
              where (x.JobNumber = jobNumber)
              take 1
              select(x)
            }
        match job|> Seq.tryHead with
        |None -> ([])
        |Some j ->
        
          query {
            for x in c.Dbo.JobOrderDetailBom do
              where (x.JobOrderKey = j.IdentityColumn)
              select (
                new JobOrderRow(
                  x.PartNumber, 
                  x.Description, 
                  x.UnitOfMeasure,
                  x.UnitPrice, 
                  x.Quantity,
                  x.RevisionNumber, 
                  j.JobNumber,
                  x.ItemNumber,
                  x.IdentityColumn
              )
                )
          }
        |> List.ofSeq


  // sql.GetDataContext().``Design Time Commands``.ClearDatabaseSchemaCache

  




