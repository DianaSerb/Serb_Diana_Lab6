using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoLotModel;
using System.Data.Entity;
using System.Data;

namespace Serb_Diana_Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }


    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        //AutoLotEntitiesModel inv = new AutoLotEntitiesModel();
        CollectionViewSource inventoryViewSource;
        CollectionViewSource customerOrdersViewSource;

        Binding custIdTextBoxBinding = new Binding();
        Binding firstNameTextBoxBinding = new Binding();
        Binding lastNameTextBoxBinding = new Binding();
        Binding carIdTextBoxBinding = new Binding();
        Binding colorTextBoxBinding = new Binding();
        Binding makeTextBoxBinding = new Binding();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            custIdTextBoxBinding.Path = new PropertyPath("CustId");
            firstNameTextBoxBinding.Path = new PropertyPath("FirstName");
            lastNameTextBoxBinding.Path = new PropertyPath("LastName");
            carIdTextBoxBinding.Path = new PropertyPath("CarId");
            colorTextBoxBinding.Path = new PropertyPath("Color");
            makeTextBoxBinding.Path = new PropertyPath("Make");
            custIdTextBox.SetBinding(TextBox.TextProperty, custIdTextBoxBinding);
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
            carIdTextBox.SetBinding(TextBox.TextProperty, carIdTextBoxBinding);
            colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;

            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource.Source = ctx.Inventories.Local;

            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            //customerOrdersViewSource.Source = ctx.Orders.Local;

            ctx.Customers.Load();
            ctx.Orders.Load();
            ctx.Inventories.Load();
            // Load data by setting the CollectionViewSource.Source property:
            // inventoryViewSource.Source = [generic data source]

            cmbCustomers.ItemsSource = ctx.Customers.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";

            cmbInventory.ItemsSource = ctx.Inventories.Local;
            //cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";
            BindDataGrid();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;

                custIdTextBox.IsEnabled = false;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }

            else
                if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;

                custIdTextBox.IsEnabled = false;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;

                custIdTextBox.SetBinding(TextBox.TextProperty, custIdTextBoxBinding);
                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
                customerViewSource.View.Refresh();
                customerViewSource.View.MoveCurrentTo(customer);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;

                custIdTextBox.IsEnabled = false;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;

                custIdTextBox.SetBinding(TextBox.TextProperty, custIdTextBoxBinding);
                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
                customerViewSource.View.Refresh();
            }

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;

            custIdTextBox.IsEnabled = true;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(custIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            custIdTextBox.Text = "";
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempCustId = custIdTextBox.Text.ToString();
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;

            custIdTextBox.IsEnabled = true;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(custIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            SetValidationBinding();
            custIdTextBox.Text = tempCustId;
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;

            Keyboard.Focus(firstNameTextBox);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempCustId = custIdTextBox.Text.ToString();
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;

            BindingOperations.ClearBinding(custIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            custIdTextBox.Text = tempCustId;
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnPrevious.IsEnabled = true;
            btnNext.IsEnabled = true;

            custIdTextBox.IsEnabled = false;
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;

            custIdTextBox.SetBinding(TextBox.TextProperty, custIdTextBoxBinding);
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);

        }

        private void btnINew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnINew.IsEnabled = false;
            btnIEdit.IsEnabled = false;
            btnIDelete.IsEnabled = false;
            btnISave.IsEnabled = true;
            btnICancel.IsEnabled = true;
            btnIPrevious.IsEnabled = false;
            btnINext.IsEnabled = false;

            carIdTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(carIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);

            carIdTextBox.Text = "";
            colorTextBox.Text = "";
            makeTextBox.Text = "";
            Keyboard.Focus(makeTextBox);
        }

        private void btnIEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempCarId = carIdTextBox.Text.ToString();
            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();

            btnINew.IsEnabled = false;
            btnIEdit.IsEnabled = false;
            btnIDelete.IsEnabled = false;
            btnISave.IsEnabled = true;
            btnICancel.IsEnabled = true;
            btnIPrevious.IsEnabled = false;
            btnINext.IsEnabled = false;

            carIdTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(carIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);

            carIdTextBox.Text = tempCarId;
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;

            Keyboard.Focus(makeTextBox);
        }

        private void btnIDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempCarId = carIdTextBox.Text.ToString();
            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();

            btnINew.IsEnabled = false;
            btnIEdit.IsEnabled = false;
            btnIDelete.IsEnabled = false;
            btnISave.IsEnabled = true;
            btnICancel.IsEnabled = true;
            btnIPrevious.IsEnabled = false;
            btnINext.IsEnabled = false;

            BindingOperations.ClearBinding(carIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);

            carIdTextBox.Text = tempCarId;
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;
        }

        private void btnISave_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            if (action == ActionState.New)
            {
                try
                {
                    inventory = new Inventory()
                    {
                        Color = colorTextBox.Text.Trim(),
                        Make = makeTextBox.Text.Trim()
                    };
                    ctx.Inventories.Add(inventory);
                    inventoryViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnINew.IsEnabled = true;
                btnIEdit.IsEnabled = true;
                btnISave.IsEnabled = false;
                btnICancel.IsEnabled = false;
                btnIPrevious.IsEnabled = true;
                btnINext.IsEnabled = true;

                carIdTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
            }

            else
                if (action == ActionState.Edit)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    inventory.Color = colorTextBox.Text.Trim();
                    inventory.Make = makeTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnINew.IsEnabled = true;
                btnIEdit.IsEnabled = true;
                btnIDelete.IsEnabled = true;
                btnISave.IsEnabled = false;
                btnICancel.IsEnabled = false;
                btnIPrevious.IsEnabled = true;
                btnINext.IsEnabled = true;

                carIdTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;

                carIdTextBox.SetBinding(TextBox.TextProperty, carIdTextBoxBinding);
                colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
                makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
                inventoryViewSource.View.Refresh();
                inventoryViewSource.View.MoveCurrentTo(inventory);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    ctx.Inventories.Remove(inventory);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnINew.IsEnabled = true;
                btnIEdit.IsEnabled = true;
                btnIDelete.IsEnabled = true;
                btnISave.IsEnabled = false;
                btnICancel.IsEnabled = false;
                btnIPrevious.IsEnabled = true;
                btnINext.IsEnabled = true;

                carIdTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;

                carIdTextBox.SetBinding(TextBox.TextProperty, carIdTextBoxBinding);
                colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
                makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
                inventoryViewSource.View.Refresh();
            }
            SetValidationBinding();
        }

        private void btnICancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnINew.IsEnabled = true;
            btnIEdit.IsEnabled = true;
            btnISave.IsEnabled = false;
            btnICancel.IsEnabled = false;
            btnIPrevious.IsEnabled = true;
            btnINext.IsEnabled = true;

            carIdTextBox.IsEnabled = false;
            colorTextBox.IsEnabled = false;
            makeTextBox.IsEnabled = false;

            carIdTextBox.SetBinding(TextBox.TextProperty, carIdTextBoxBinding);
            colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
        }

        private void btnIPrevious_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();

        }

        private void btnINext_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }

        private void btnOSave_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;

                    order = new Order()
                    {
                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();
                    ctx.SaveChanges();
                }


                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnONew.IsEnabled = true;
                btnOEdit.IsEnabled = true;
                btnOSave.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnOCancel.IsEnabled = false;
                btnOPrevious.IsEnabled = true;
                btnONext.IsEnabled = true;

            }

            else
                if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnONew.IsEnabled = true;
                btnOEdit.IsEnabled = true;
                btnODelete.IsEnabled = true;
                btnOSave.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnOCancel.IsEnabled = false;
                btnOPrevious.IsEnabled = true;
                btnONext.IsEnabled = true;

                BindDataGrid();
                customerOrdersViewSource.View.Refresh();
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnONew.IsEnabled = true;
                btnOEdit.IsEnabled = true;
                btnODelete.IsEnabled = true;
                btnOSave.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnOCancel.IsEnabled = false;
                btnOPrevious.IsEnabled = true;
                btnONext.IsEnabled = true;
            }
        }

        private void btnOPrevious_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnONext_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }

        private void btnONew_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnONew.IsEnabled = true;
            btnOEdit.IsEnabled = true;
            btnOSave.IsEnabled = false;
            btnOCancel.IsEnabled = false;
            btnOPrevious.IsEnabled = true;
            btnONext.IsEnabled = true;
        }

        private void btnODelete_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals cust.CustId
                             join inv in ctx.Inventories on ord.CarId equals inv.CarId
                             select new { ord.OrderId, ord.CarId, ord.CustId, cust.FirstName, cust.LastName, inv.Make, inv.Color };
            customerOrdersViewSource.Source = queryOrder.ToList();

        }
    }
    
}
