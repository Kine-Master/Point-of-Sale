Imports System.Data.SQLite

Public Class DatabaseDisplayForm
    Private mainForm As Form1
    Private connectionString As String

    Public Sub DatabaseDisplayForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadOrders()
        Me.BackColor = Color.FromArgb(210, 180, 140)
    End Sub

    Public Sub LoadOrders()
        Try
            Using connection As New SQLiteConnection(connectionString)
                connection.Open()

                Dim query As String = "SELECT OrderID, ItemName, Quantity, Price, TotalAmount, OrderDate FROM OrderDetails"
                Using cmd As New SQLiteCommand(query, connection)
                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        Dim dt As New DataTable()
                        dt.Load(reader)
                        DataGridView1.DataSource = dt
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading orders: " & ex.Message)
        End Try
    End Sub

    Private Sub btnResetTable_Click(sender As Object, e As EventArgs) Handles btnResetTable.Click
        Dim result = MessageBox.Show("Are you sure you want to reset the entire table?", "Confirm Reset", MessageBoxButtons.YesNo)
        If result = DialogResult.Yes Then
            Using conn As New SQLiteConnection(connectionString)
                conn.Open()
                Dim command As New SQLiteCommand("DELETE FROM OrderDetails", conn) ' Adjusted table name to OrderDetails
                command.ExecuteNonQuery()
            End Using
            ' Refresh the DataGridView
            DatabaseDisplayForm_Load(Nothing, Nothing)
        End If
    End Sub

    Private Sub btnDeleteEntry_Click(sender As Object, e As EventArgs) Handles btnDeleteEntry.Click
        If DataGridView1.SelectedRows.Count > 0 Then
            Dim orderId As Integer = Convert.ToInt32(DataGridView1.SelectedRows(0).Cells("OrderID").Value) ' Adjust "OrderID" to your actual primary key column name
            Using conn As New SQLiteConnection(connectionString)
                conn.Open()
                Dim command As New SQLiteCommand("DELETE FROM OrderDetails WHERE OrderID = @OrderID", conn) ' Adjusted table name and column name
                command.Parameters.AddWithValue("@OrderID", orderId)
                command.ExecuteNonQuery()
            End Using
            ' Refresh the DataGridView
            DatabaseDisplayForm_Load(Nothing, Nothing)
        Else
            MessageBox.Show("Please select a row to delete.")
        End If
    End Sub


    Public Sub New(connString As String, parentForm As Form1)
        InitializeComponent()
        connectionString = connString
        mainForm = parentForm
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        mainForm.Show()
        Me.Hide()
    End Sub
End Class