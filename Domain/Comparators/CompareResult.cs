using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;

public class RecordComparator
{
       public ComparisonResult Compare(InternalRecord internalRecord, ExternalRecord externalRecord)

    {
        if(internalRecord.TransactionId != externalRecord.TransactionId)
        {
            return new ComparisonResult(

                internalRecord,
                externalRecord,
                internalRecord.TransactionId,
                RecordCompare.Unavailable,
                "Transaction differs"
            );
        }



        if(internalRecord.Amount != externalRecord.Amount)
        {
            return new ComparisonResult(
                internalRecord,
                externalRecord,
                internalRecord.TransactionId,
                RecordCompare.Mismatch,
                "Amount difference"

            );
        }

        if(internalRecord.SenderId != externalRecord.SenderId)
        {
            return new ComparisonResult(
                  internalRecord,
                  externalRecord,
                  internalRecord.TransactionId,
                  RecordCompare.Mismatch,
                  "Senderid differs"

            );
        }

        if(internalRecord.Currency != externalRecord.Currency)
        {
            return new ComparisonResult(
                internalRecord,
                externalRecord,
                internalRecord.TransactionId,
                RecordCompare.Mismatch,
                "Currency differs"
            );
        }

        return new ComparisonResult(

          internalRecord,
          externalRecord,
          internalRecord.TransactionId,
          RecordCompare.Match,
          null


        );

        





    }
}