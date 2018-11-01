﻿namespace FsFirestore

module Transaction =
    
    open Google.Cloud.Firestore
    open FsFirestore.Firestore
    open FsFirestore.Utils
    open FsFirestore.TransUtils

    /// Runs a given transaction function in a Firestore transaction.
    let runTransaction<'T when 'T : not struct> (transactionFunc: Transaction -> 'T) (options: TransactionOptions option)  =
        runTransaction<'T> db transactionFunc options

    /// Executes a given query with respect to the given transaction.
    let execQueryInTrans<'T when 'T : not struct> (transaction: Transaction) (query: Query) =
        (getQuerySnapshotInTrans transaction query).Documents
        |> deserializeSnapshots<'T>

    /// Returns a document from a collection with respect to the given transaction.
    let documentInTrans<'T when 'T : not struct> (transaction: Transaction) col id =
        documentRef col id
        |> getDocSnapshotInTrans transaction
        |> convertSnapshotTo<'T>

    /// Returns a list of documents from a collection with respect to a given transaction
    let documentsInTrans<'T when 'T : not struct>  (transaction: Transaction) col ids =
       documentRefs col ids
       |> getDocSnapshotsInTrans transaction
       |> deserializeSnapshots<'T>

    /// Adds a document to a collection with respect to the given transaction.
    let addDocumentInTrans (transaction: Transaction) col id data =
        collection col
        |> createDocInTrans transaction id data

    /// Updates a document in a collection with respect to the given transaction.
    let updateDocumentInTrans (transaction: Transaction) col id data =
        collection col 
        |> setDocInTrans transaction id data

    /// Deletes a document in a collection with respect to the given transaction.
    let deleteDocumentInTrans (transaction: Transaction) (precondition: Precondition option) col id =
        collection col
        |> deleteDocTrans transaction precondition id
        
    /// Deletes multiple documents in a collection with respect to the given transaction.
    let deleteDocumentsInTrans (transaction: Transaction) (precondition: Precondition option) col (ids: string seq)  =
        ids
        |>Seq.iter (deleteDocumentInTrans transaction precondition col)