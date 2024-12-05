open System
open System.Windows.Forms
open StudentsDb

// Create a Windows Form to display students
let CreateForm () =
    // Load students
    let students = loadStudents()

    // Create form
    let form = new Form(Text = "Student Management System", Width = 800, Height = 600)

    // Create DataGridView to display student data
    let dataGridView = new DataGridView(Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill)
    dataGridView.DataSource <- students |> List.toArray

    // Add the DataGridView to the form
    form.Controls.Add(dataGridView)

    form

[<EntryPoint>]
let main argv =
    let form = CreateForm()
    try
        Application.Run(form)
    with
    | :? InvalidOperationException -> printfn "Invalid Operation"
    | (ex: exn) -> printfn "Exception occurred: %s" ex.Message
    0

