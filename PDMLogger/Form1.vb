Imports System.Data.SqlClient
Public Class Form1
    Public myDB As New SqlConnection
    Dim connString As String = "Server=LPDM;Database=EPDM_Reporting;User ID=EPDMReporting;Password=Reporting1;MultipleActiveResultSets=true;"
    'EPDM_Reporting
    'Reporting1

    Public Sub CheckDB()

    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'Timer1.Enabled = False
        DoReport()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        myDB.Close()
    End Sub
    Sub DoReport()
        If ProgressBar1.Value = ProgressBar1.Maximum Then
            ProgressBar1.Value = 0
        End If
        Dim mySW As New IO.StreamWriter("C:\Users\joel.moxley\Dropbox\Debouncer\epdmupdate.txt", True)
        ProgressBar1.Increment(1)
        ListBox1.Items.Clear()
        Dim myComm As New SqlCommand("update EPDM_Reporting.dbo.Revisions set datereported = '" & DateTime.Now & "', reportstatus = 0 where documentid is null", myDB)
        myComm.ExecuteNonQuery()
        myComm.CommandText = "Select count(*) from Revisions where datereported is null"
        If myComm.ExecuteScalar > 0 Then
            myComm.CommandText = "Select * from Revisions where datereported is null and documentid is not null order by UniqueID"
            Dim myReader As SqlDataReader = myComm.ExecuteReader
            Dim myRevs As New List(Of rev)
            While myReader.Read
                Dim myRev As New rev(myReader("documentid"), myReader("revnr"), myReader("dateadded"), myReader("uniqueid"))
                myRevs.Add(myRev)
            End While
            myReader.Close()
            For Each myRev As rev In myRevs
                Dim buildString As String = "EpdmReport: " & myRev.revNr & ": " & myRev.dateAdded & ": " & myRev.UserName(myDB) & ": " & myRev.filePath(myDB)
                'your code here
                ListBox1.Items.Add(buildString)
                mySW.WriteLine(buildString)
                myComm.CommandText = "Update EPDM_Reporting.dbo.Revisions Set datereported = '" & DateTime.Now & "', reportstatus = 1 where UniqueID =" & myRev.UniqueID
                myComm.ExecuteNonQuery()
            Next
        End If
        mySW.Close()
        mySW.Dispose()
        'documentid
        'revnr
        'dateadded
        'datereported
        'reportstatus

    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            myDB.ConnectionString = connString
            myDB.Open()
        Catch ex As System.Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        DoReport()
    End Sub
End Class
Public Class rev
    Public UniqueID As Integer
    Public documentID As Integer
    Public revNr As Integer
    Public dateAdded As Date
    Public Sub New(DocID As Integer, RevNumber As Integer, dateAdd As Date, ID As Integer)
        documentID = DocID
        revNr = RevNumber
        dateAdded = dateAdd
        UniqueID = ID
    End Sub
    Public Function UserName(dbconn As SqlConnection) As String
        Dim myComm As New SqlCommand("Select UserName from Foro_PDM_Vault.dbo.Users where UserID = (Select UserID from Foro_PDM_Vault.dbo.Revisions where documentid = " & documentID & " and RevNr = " & revNr & ")", dbconn)
        Return myComm.ExecuteScalar
    End Function
    Public Function filePath(dbConn As SqlConnection) As String
        Dim myComm As New SqlCommand("Select path from foro_pdm_vault.dbo.Projects Where ProjectID = (Select ProjectID from Foro_PDM_Vault.dbo.DocumentsInProjects where documentid = " & documentID & " and Deleted = 0)", dbConn)
        Dim pathVal As String = myComm.ExecuteScalar
        myComm.CommandText = "Select filename from Foro_PDM_Vault.dbo.Documents where Documentid = " & documentID
        Dim fileName As String = myComm.ExecuteScalar
        Return "C:\Foro_PDM_Vault" & pathVal & fileName
    End Function

End Class