namespace bomLite

type ItemMaster(partNumber,description,unitOfMeasure,unitPrice,revisionNumber) =
  member val PartNumber:string = partNumber with get, set 
  member val Description:string = description with get, set
  member val UnitOfMeasure:string =  unitOfMeasure with get, set
  member val UnitPrice :decimal= unitPrice with get, set 
  member val RevisionNumber:string = revisionNumber with get, set 
  member val IdentityColumn:int = identity with get, set 

type JobOrderMaster(partNumber,description,unitOfMeasure,unitPrice,quantity,revisionNumber,jobNumber) =
  member val JobNumber:string = jobNumber with get, set 
  member val PartNumber:string = partNumber with get, set 
  member val Description:string = description with get, set
  member val UnitOfMeasure:string =  unitOfMeasure with get, set
  member val UnitPrice :decimal= unitPrice with get, set 
  member val Quantity :decimal=   quantity with get, set 
  member val RevisionNumber:string = revisionNumber with get, set 
  member val IdentityColumn:int = identity with get, set 

type JobOrderRow(partNumber,description,unitOfMeasure,unitPrice,quantity,revisionNumber,jobNumber,itemNumber) =
  member val ItemNumber:string = itemNumber with get, set 
  member val JobNumber:string = jobNumber with get, set 
  member val PartNumber:string = partNumber with get, set 
  member val Description:string = description with get, set
  member val UnitOfMeasure:string =  unitOfMeasure with get, set
  member val UnitPrice :decimal= unitPrice with get, set 
  member val Quantity :float=   quantity with get, set 
  member val RevisionNumber:string = revisionNumber with get, set 
  member val IdentityColumn:int = identity with get, set 
