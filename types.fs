namespace bomLite
open System

type ItemMaster(partNumber,description,unitOfMeasure,unitPrice,revisionNumber,identity) =
  member val PartNumber:string = partNumber with get, set 
  member val Description:string = description with get, set
  member val UnitOfMeasure:string =  unitOfMeasure with get, set
  member val UnitPrice :decimal= unitPrice with get, set 
  member val RevisionNumber:string = revisionNumber with get, set 
  member val IdentityColumn:int = identity with get, set 

type JobOrderMaster(jobNumber,description,salesOrderNumber,parentJobNumber,hasSubJobs,companyName,orderDate,dueDate,status,identity) =
  member val JobNumber:string = jobNumber with get, set 
  member val Description:string = description with get, set
  member val SalesOrder:string = salesOrderNumber with get, set 
  member val ParentJobNumber:string = parentJobNumber with get, set 
  member val HasSubJobs :bool=   hasSubJobs with get, set 
  member val CompanyName:string = companyName with get, set 
  member val OrderDate:DateTime = orderDate with get, set 
  member val DueDate:DateTime = dueDate with get, set 
  member val Status:string = status with get, set 
  member val IdentityColumn:int = identity with get, set 

type JobOrderRow(partNumber,description,unitOfMeasure,unitPrice,quantity,revisionNumber,jobNumber,itemNumber,identity) =
  member val ItemNumber:string = itemNumber with get, set 
  member val JobNumber:string = jobNumber with get, set 
  member val PartNumber:string = partNumber with get, set 
  member val Description:string = description with get, set
  member val UnitOfMeasure:string =  unitOfMeasure with get, set
  member val UnitPrice :decimal= unitPrice with get, set 
  member val Quantity :float=   quantity with get, set 
  member val RevisionNumber:string = revisionNumber with get, set 
  member val IdentityColumn:int = identity with get, set 
