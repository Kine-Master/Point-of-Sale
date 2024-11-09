Imports System.Data.SQLite

Public Class Form1
    Private Shared databaseDisplayForm As DatabaseDisplayForm

    Private databasePath As String = System.IO.Path.Combine(Application.StartupPath, "posDatabase.db")
    Private connectionString As String = $"Data Source={databasePath};Version=3;"
    Private connection As SQLiteConnection

    Private orderItems As New Dictionary(Of String, Decimal)
    Private totalAmount As Decimal = 0


    Private Sub InitializeDatabase()
        ' Check if the database file exists, and if not, create it
        If Not System.IO.File.Exists(databasePath) Then
            SQLiteConnection.CreateFile(databasePath)
            MessageBox.Show("Database file created successfully.")
        End If

        ' Create tables if they do not exist
        Using connection As New SQLiteConnection(connectionString)
            connection.Open()
            Dim createOrderTableQuery As String = "
                CREATE TABLE IF NOT EXISTS OrderDetails (
                    OrderID INTEGER PRIMARY KEY AUTOINCREMENT,
                    ItemName TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Price REAL NOT NULL,
                    TotalAmount REAL NOT NULL,
                    OrderDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );"

            Using command As New SQLiteCommand(createOrderTableQuery, connection)
                command.ExecuteNonQuery()
            End Using
            connection.Close()
        End Using
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.BackColor = Color.FromArgb(210, 180, 140)

        Try
            InitializeDatabase()
        Catch ex As Exception
            MessageBox.Show("Database initialization error: " & ex.Message)
        End Try

        pnlCoffeeItems.BackColor = Color.Beige
        pnlTeaItems.BackColor = Color.Beige
        pnlBestSellersItems.BackColor = Color.Beige
        pnlAddOns.BackColor = Color.Beige
        pnlCategories.BackColor = Color.Beige
        pnlHeader.BackColor = Color.Beige
        pnlOrderDetails.BackColor = Color.Beige
        pnlCheckout.BackColor = Color.Beige
        pnlItems.BackColor = Color.Beige
        pnlCoffeeItems.Visible = True
        pnlTeaItems.Visible = False
        pnlBestSellersItems.Visible = False
        pnlAddOns.Visible = True

        SetMenuItem(lblBestSellerMenu1, "Chicken Alfredo;250")
        SetMenuItem(lblBestSellerMenu2, "Lasagna;250")
        SetMenuItem(lblBestSellerMenu3, "Italian Spagehetti;250")
        SetMenuItem(lblBestSellerMenu4, "Hawaiian Pizza;250")
        SetMenuItem(lblBestSellerMenu5, "Parfait;250")
        SetMenuItem(lblBestSellerMenu6, "Mango Mousse;250")
        SetMenuItem(lblTeaMenu1, "Iced Tea;50")
        SetMenuItem(lblTeaMenu2, "Green Tea;50")
        SetMenuItem(lblTeaMenu3, "Oolong Tea;50")
        SetMenuItem(lblTeaMenu4, "Chamomile Tea;50")
        SetMenuItem(lblTeaMenu5, "Matcha;50")
        SetMenuItem(lblTeaMenu6, "Lemon Tea;50")
        SetMenuItem(lblCoffeeMenu1, "Latte;150")
        SetMenuItem(lblCoffeeMenu2, "Cappuccino;150")
        SetMenuItem(lblCoffeeMenu3, "Macchiato;150")
        SetMenuItem(lblCoffeeMenu4, "Espresso;150")
        SetMenuItem(lblCoffeeMenu5, "Affogato;150")
        SetMenuItem(lblCoffeeMenu6, "Americano;150")
        SetMenuItem(lblAddOnMenu1, "Coffee;10")
        SetMenuItem(lblAddOnMenu2, "Milk;20")
        SetMenuItem(lblAddOnMenu3, "Ice;5")
        SetMenuItem(lblAddOnMenu4, "Honey;10")
    End Sub



    Private Sub OpenConnection()
        If connection Is Nothing Then
            connection = New SQLiteConnection(connectionString)
        End If
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If
    End Sub

    Private Sub CloseConnection()
        If connection IsNot Nothing AndAlso connection.State = ConnectionState.Open Then
            connection.Close()
        End If
    End Sub

    Private Sub SaveOrderToDatabase(itemName As String, quantity As Integer, price As Decimal, totalAmount As Decimal)
        Try
            OpenConnection()
            Dim query As String = "INSERT INTO OrderDetails (ItemName, Quantity, Price, TotalAmount) VALUES (@ItemName, @Quantity, @Price, @TotalAmount)"
            Using cmd As New SQLiteCommand(query, connection)
                cmd.Parameters.AddWithValue("@ItemName", itemName)
                cmd.Parameters.AddWithValue("@Quantity", quantity)
                cmd.Parameters.AddWithValue("@Price", price)
                cmd.Parameters.AddWithValue("@TotalAmount", totalAmount)
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            MessageBox.Show("Error saving order: " & ex.Message)
        Finally
            CloseConnection()
        End Try
    End Sub


    Private Sub btnCheckout_Click(sender As Object, e As EventArgs) Handles btnCheckout.Click
        If orderItems.Count = 0 Then
            MessageBox.Show("No items in the order to checkout.")
            Return
        End If

        ' Save each item to the database
        For Each item In orderItems
            Dim itemName As String = item.Key
            Dim itemPrice As Decimal = item.Value
            Dim itemParts As String() = itemName.Split(" x ")
            Dim itemQuantity As Integer = Convert.ToInt32(itemParts(1))

            SaveOrderToDatabase(itemParts(0).Trim(), itemQuantity, itemPrice / itemQuantity, itemPrice)
        Next

        ' Clear order details after checkout
        orderItems.Clear()
        lstOrderDetails.Items.Clear()
        totalAmount = 0
        lblTotalAmount.Text = "Total: Php 0.00"
        txtCashAmount.Text = ""
        lblChange.Text = "Change: Php 0.00"

        MessageBox.Show("Checkout successful and order saved to database.")
    End Sub

    Private Sub DisplayOrders()
        Try
            OpenConnection()
            Dim query As String = "SELECT * FROM OrderDetails"
            Using cmd As New SQLiteCommand(query, connection)
                Using reader As SQLiteDataReader = cmd.ExecuteReader()
                    Dim orders As String = "Order Details:" & vbCrLf
                    While reader.Read()
                        Dim orderID = reader("OrderID").ToString()
                        Dim itemName = reader("ItemName").ToString()
                        Dim quantity = reader("Quantity").ToString()
                        Dim price = reader("Price").ToString()
                        Dim totalAmount = reader("TotalAmount").ToString()
                        Dim orderDate = reader("OrderDate").ToString()
                        orders &= $"OrderID: {orderID}, Item: {itemName}, Quantity: {quantity}, Price: {price}, Total: {totalAmount}, Date: {orderDate}" & vbCrLf
                    End While
                    MessageBox.Show(orders)
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error displaying orders: " & ex.Message)
        Finally
            CloseConnection()
        End Try
    End Sub


    Private Sub SetMenuItem(label As Label, tag As String)
        label.Tag = tag
        AddHandler label.Click, AddressOf MenuItem_Click
    End Sub

    Private Sub MenuItem_Click(sender As Object, e As EventArgs)
        Dim label As Label = CType(sender, Label)
        Dim tagParts As String() = label.Tag.ToString().Split(";"c)

        If tagParts.Length = 2 Then
            Dim itemName As String = tagParts(0).Trim()
            Dim itemPrice As Decimal
            If Decimal.TryParse(tagParts(1).Trim(), itemPrice) Then
                ' Prompt for quantity
                Dim quantity As String = InputBox("Enter the quantity for " & itemName, "Quantity")
                Dim itemQuantity As Integer
                If Integer.TryParse(quantity, itemQuantity) AndAlso itemQuantity > 0 Then
                    ' Add item with quantity to order
                    Dim totalPrice As Decimal = itemPrice * itemQuantity
                    AddToOrder(itemName & " x " & itemQuantity, totalPrice)
                Else
                    MessageBox.Show("Please enter a valid quantity.")
                End If
            End If
        End If
    End Sub

    Private Sub AddToOrder(itemName As String, itemPrice As Decimal)
        ' Add the item to the dictionary and update total amount
        orderItems(itemName) = itemPrice
        totalAmount += itemPrice

        ' Display the item in the ListBox
        lstOrderDetails.Items.Add($"{itemName} - Php {itemPrice:F2}")

        ' Update the total amount label
        lblTotalAmount.Text = $"Total Amount: Php {totalAmount:F2}"
    End Sub

    Private Sub RemoveFromOrder()
        If lstOrderDetails.SelectedIndex >= 0 Then
            ' Get the selected item name
            Dim selectedItem = lstOrderDetails.SelectedItem.ToString()
            Dim itemParts = selectedItem.Split(" - ")
            Dim itemName = itemParts(0).Trim() ' Get the item name and remove extra spaces

            ' Ensure the item exists in the dictionary before attempting to remove
            If orderItems.ContainsKey(itemName) Then
                ' Update total amount and remove the item from the dictionary
                totalAmount -= orderItems(itemName)
                orderItems.Remove(itemName)
                lstOrderDetails.Items.RemoveAt(lstOrderDetails.SelectedIndex)

                ' Update the total amount label
                lblTotalAmount.Text = $"Total: Php {totalAmount:F2}" ' Correct currency label
            Else
                MessageBox.Show("Item not found in the order.")
            End If
        Else
            MessageBox.Show("Please select an item to remove.")
        End If
    End Sub

    Private Sub btnCalculateChange_Click(sender As Object, e As EventArgs) Handles btnCalculateChange.Click
        Dim cashAmount As Decimal

        ' Check if the entered cash amount is a valid decimal
        If Decimal.TryParse(txtCashAmount.Text, cashAmount) Then
            If cashAmount >= totalAmount Then
                Dim change As Decimal = cashAmount - totalAmount
                lblChange.Text = $"Change: Php {change:F2}"
            Else
                MessageBox.Show("The cash amount entered is less than the total amount. Please enter a valid amount.")
            End If
        Else
            MessageBox.Show("Please enter a valid cash amount.")
        End If
    End Sub


    Private Sub lstOrderDetails_MouseClick(sender As Object, e As MouseEventArgs) Handles lstOrderDetails.MouseClick
        If lstOrderDetails.SelectedIndex >= 0 Then
            RemoveFromOrder() ' Call the method to remove the selected item
        End If
    End Sub

    Private Sub lblCheckout_Click(sender As Object, e As EventArgs) Handles lblCheckout.Click

    End Sub

    Private Sub pnlCategories_Paint(sender As Object, e As PaintEventArgs) Handles pnlCategories.Paint
        Dim borderColor As Color = Color.Brown
        Dim borderWidth As Integer = 2
        ControlPaint.DrawBorder(e.Graphics, pnlCategories.ClientRectangle, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid)
    End Sub

    Private Sub pnlHeader_Paint(sender As Object, e As PaintEventArgs) Handles pnlHeader.Paint
        Dim borderColor As Color = Color.Brown
        Dim borderWidth As Integer = 2
        ControlPaint.DrawBorder(e.Graphics, pnlHeader.ClientRectangle, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid)
    End Sub

    Private Sub pnlOrderDetails_Paint(sender As Object, e As PaintEventArgs) Handles pnlOrderDetails.Paint
        Dim borderColor As Color = Color.Brown
        Dim borderWidth As Integer = 2
        ControlPaint.DrawBorder(e.Graphics, pnlOrderDetails.ClientRectangle, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid)
    End Sub

    Private Sub pnlCheckout_Paint(sender As Object, e As PaintEventArgs) Handles pnlCheckout.Paint
        Dim borderColor As Color = Color.Brown
        Dim borderWidth As Integer = 2
        ControlPaint.DrawBorder(e.Graphics, pnlCheckout.ClientRectangle, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid)
    End Sub

    Private Sub pnlItems_Paint(sender As Object, e As PaintEventArgs) Handles pnlItems.Paint
        Dim borderColor As Color = Color.Brown
        Dim borderWidth As Integer = 2
        ControlPaint.DrawBorder(e.Graphics, pnlItems.ClientRectangle, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid)
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub lblItems_Click(sender As Object, e As EventArgs) Handles lblItems.Click

    End Sub

    Private Sub lblHeading_Click(sender As Object, e As EventArgs) Handles lblHeading.Click

    End Sub

    Private Sub lblOrderDetails_Click(sender As Object, e As EventArgs) Handles lblOrderDetails.Click

    End Sub

    Private Sub btnCoffees_Click(sender As Object, e As EventArgs) Handles btnCoffees.Click
        pnlCoffeeItems.Visible = True
        pnlTeaItems.Visible = False
        pnlBestSellersItems.Visible = False
        pnlAddOns.Visible = True
    End Sub

    Private Sub btnTea_Click(sender As Object, e As EventArgs) Handles btnTea.Click
        pnlCoffeeItems.Visible = False
        pnlTeaItems.Visible = True
        pnlBestSellersItems.Visible = False
        pnlAddOns.Visible = True
    End Sub

    Private Sub btnBestSellers_Click(sender As Object, e As EventArgs) Handles btnBestSellers.Click
        pnlCoffeeItems.Visible = False
        pnlTeaItems.Visible = False
        pnlBestSellersItems.Visible = True
        pnlAddOns.Visible = False
    End Sub

    Private Sub pnlCoffeeItems_Paint(sender As Object, e As PaintEventArgs) Handles pnlCoffeeItems.Paint
        Dim borderColor As Color = Color.Brown
        Dim borderWidth As Integer = 2
        ControlPaint.DrawBorder(e.Graphics, pnlCoffeeItems.ClientRectangle, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid)
    End Sub
    Private Sub pnlTeaItems_Paint(sender As Object, e As PaintEventArgs) Handles pnlTeaItems.Paint
        Dim borderColor As Color = Color.Brown
        Dim borderWidth As Integer = 2
        ControlPaint.DrawBorder(e.Graphics, pnlTeaItems.ClientRectangle, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid)
    End Sub
    Private Sub pnlBestSellersItems_Paint(sender As Object, e As PaintEventArgs) Handles pnlBestSellersItems.Paint
        Dim borderColor As Color = Color.Brown
        Dim borderWidth As Integer = 2
        ControlPaint.DrawBorder(e.Graphics, pnlBestSellersItems.ClientRectangle, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid)
    End Sub

    Private Sub pnlAddOns_Paint(sender As Object, e As PaintEventArgs) Handles pnlAddOns.Paint
        Dim borderColor As Color = Color.Brown
        Dim borderWidth As Integer = 2
        ControlPaint.DrawBorder(e.Graphics, pnlAddOns.ClientRectangle, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid, borderColor, borderWidth, ButtonBorderStyle.Solid)
    End Sub




    Private Sub lblAddOnMenuPrice1_Click(sender As Object, e As EventArgs) Handles lblAddOnMenuPrice1.Click

    End Sub
    Private Sub lblAddOnMenuPrice2_Click(sender As Object, e As EventArgs) Handles lblAddOnMenuPrice2.Click

    End Sub
    Private Sub lblAddOnMenuPrice3_Click(sender As Object, e As EventArgs)

    End Sub
    Private Sub lblAddOnMenuPrice4_Click(sender As Object, e As EventArgs) Handles lblAddOnMenuPrice4.Click

    End Sub
    Private Sub lblAddOnMenu1_Click(sender As Object, e As EventArgs) Handles lblAddOnMenu1.Click

    End Sub

    Private Sub lblAddOnMenu2_Click(sender As Object, e As EventArgs) Handles lblAddOnMenu2.Click

    End Sub

    Private Sub lblAddOnMenu3_Click(sender As Object, e As EventArgs) Handles lblAddOnMenu3.Click

    End Sub

    Private Sub lblAddOnMenu4_Click(sender As Object, e As EventArgs) Handles lblAddOnMenu4.Click

    End Sub




    Private Sub lblBestSellerMenuPrice1_Click(sender As Object, e As EventArgs) Handles lblBestSellerMenuPrice1.Click

    End Sub

    Private Sub lblBestSellerMenu1_Click(sender As Object, e As EventArgs) Handles lblBestSellerMenu1.Click

    End Sub

    Private Sub lblBestSellerMenu2_Click(sender As Object, e As EventArgs) Handles lblBestSellerMenu2.Click

    End Sub

    Private Sub lblBestSellerMenu3_Click(sender As Object, e As EventArgs) Handles lblBestSellerMenu3.Click

    End Sub

    Private Sub lblBestSellerMenu4_Click(sender As Object, e As EventArgs) Handles lblBestSellerMenu4.Click

    End Sub

    Private Sub lblBestSellerMenu5_Click(sender As Object, e As EventArgs) Handles lblBestSellerMenu5.Click

    End Sub

    Private Sub lblBestSellerMenu6_Click(sender As Object, e As EventArgs) Handles lblBestSellerMenu6.Click

    End Sub

    Private Sub lblTeaMenu1_Click(sender As Object, e As EventArgs) Handles lblTeaMenu1.Click

    End Sub

    Private Sub lblCoffeeMenu5_Click(sender As Object, e As EventArgs) Handles lblCoffeeMenu5.Click

    End Sub

    Private Sub lblCoffeeMenu6_Click(sender As Object, e As EventArgs) Handles lblCoffeeMenu6.Click

    End Sub

    Private Sub lblCoffeeMenu2_Click(sender As Object, e As EventArgs) Handles lblCoffeeMenu2.Click

    End Sub

    Private Sub btnShowOrders_Click(sender As Object, e As EventArgs) Handles btnShowOrders.Click
        If databaseDisplayForm Is Nothing OrElse databaseDisplayForm.IsDisposed Then
            ' Create a new instance of the form if it doesn't exist or has been disposed
            databaseDisplayForm = New DatabaseDisplayForm(connectionString, Me)
        End If

        ' Show the existing form
        databaseDisplayForm.Show()
        ' Optionally, you can bring the form to the front to make it the active window
        databaseDisplayForm.BringToFront()

        ' Optionally, you can populate the form with the order details
        databaseDisplayForm.LoadOrders()
    End Sub
End Class
