<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DatabaseDisplayForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        DataGridView1 = New DataGridView()
        btnResetTable = New Button()
        btnDeleteEntry = New Button()
        btnExit = New Button()
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' DataGridView1
        ' 
        DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridView1.Location = New Point(12, 12)
        DataGridView1.Name = "DataGridView1"
        DataGridView1.Size = New Size(776, 336)
        DataGridView1.TabIndex = 0
        ' 
        ' btnResetTable
        ' 
        btnResetTable.Location = New Point(12, 365)
        btnResetTable.Name = "btnResetTable"
        btnResetTable.Size = New Size(75, 23)
        btnResetTable.TabIndex = 1
        btnResetTable.Text = "Reset"
        btnResetTable.UseVisualStyleBackColor = True
        ' 
        ' btnDeleteEntry
        ' 
        btnDeleteEntry.Location = New Point(93, 365)
        btnDeleteEntry.Name = "btnDeleteEntry"
        btnDeleteEntry.Size = New Size(75, 23)
        btnDeleteEntry.TabIndex = 2
        btnDeleteEntry.Text = "Delete"
        btnDeleteEntry.UseVisualStyleBackColor = True
        ' 
        ' btnExit
        ' 
        btnExit.Location = New Point(713, 365)
        btnExit.Name = "btnExit"
        btnExit.Size = New Size(75, 23)
        btnExit.TabIndex = 3
        btnExit.Text = "Exit"
        btnExit.UseVisualStyleBackColor = True
        ' 
        ' DatabaseDisplayForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(btnExit)
        Controls.Add(btnDeleteEntry)
        Controls.Add(btnResetTable)
        Controls.Add(DataGridView1)
        Name = "DatabaseDisplayForm"
        Text = "DatabaseDisplayForm"
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents btnResetTable As Button
    Friend WithEvents btnDeleteEntry As Button
    Friend WithEvents btnExit As Button
End Class
