module UI
open System
open System.Windows.Forms
open System.Drawing
open Models
open Utilities
open StudentsDb

let createAdminForm() =
    let form = new Form(Text = "Admin Dashboard", ClientSize = System.Drawing.Size(400, 300))

    let addButton = new Button(Text = "Add Student", Location = System.Drawing.Point(50, 50), Size = System.Drawing.Size(100, 30))
    addButton.Click.Add(fun _ -> 
        let students = loadStudents()
        let idInput = createInputDialog "Enter ID:" ""
        match System.Int32.TryParse(idInput) with
        | (true, id) ->
            let nameInput = createInputDialog "Enter Name:" ""
            let gradesInput = createInputDialog "Enter Grades in order (Math, Science, Arabic, English) separated by commas:" ""
            let passwordInput = createInputDialog "Enter Student Password:" ""  // Password input
            if idInput <> "" && nameInput <> "" && gradesInput <> "" && passwordInput <> "" then
            
                let name = nameInput
                let grades = gradesInput.Split(',') |> Array.map int |> Array.toList
                let password = passwordInput
                let newStudent = { ID = id; Name = name; Grades = grades; Password = password }
                let newstudents = addStudent newStudent students
                saveStudents newstudents
                MessageBox.Show("Student added successfully!") |> ignore
        | _ -> 
        MessageBox.Show("Invalid ID! Please enter a numeric value.") |> ignore

    )
    form.Controls.Add(addButton)

    // Show Students
    let showButton = new Button(Text = "Show Students", Location = System.Drawing.Point(50, 100), Size = System.Drawing.Size(100, 30))
    showButton.Click.Add(fun _ ->
        let students = loadStudents()
        let studentList = students |> List.map (fun s -> sprintf "ID: %d, Name: %s, Grades: %A" s.ID s.Name s.Grades)
        MessageBox.Show(String.Join("\n", studentList)) |> ignore
    )
    form.Controls.Add(showButton)

    // Update Student
    let updateButton = new Button(Text = "Update Student", Location = System.Drawing.Point(50, 150), Size = System.Drawing.Size(100, 30))
    updateButton.Click.Add(fun _ ->
        let students = loadStudents()
        let idInput = createInputDialog "Enter Student ID to update:" ""
        match System.Int32.TryParse(idInput) with
        | (true, id) ->
            if idInput <> "" then
                let studentOpt = students |> List.tryFind (fun s -> s.ID = id)
                match studentOpt with
                | Some student -> 
                    let newName = createInputDialog "Enter new Name:" student.Name
                    let newGrades = createInputDialog "Enter Grades in order (Math, Science, Arabic, English) separated by commas:"(String.Join(",", student.Grades))

                    if newName <> "" && newGrades <> "" then
                        let updatedStudent = { student with Name = newName; Grades = newGrades.Split(',') |> Array.map int |> Array.toList }
                        let updatedstudents = editStudent student.ID updatedStudent students  // Corrected to use the new editStudent function
                        saveStudents updatedstudents
                        MessageBox.Show("Student updated successfully!") |> ignore
                | None -> MessageBox.Show("Student not found.") |> ignore
        | _ -> 
        MessageBox.Show("Invalid ID! Please enter a numeric value.") |> ignore
    )
    form.Controls.Add(updateButton)

    // Delete Student
    let deleteButton = new Button(Text = "Delete Student", Location = System.Drawing.Point(50, 200), Size = System.Drawing.Size(100, 30))
    deleteButton.Click.Add(fun _ ->
        let students = loadStudents()
        let idInput = createInputDialog "Enter Student ID to delete:" ""
        match System.Int32.TryParse(idInput) with
        | (true, id) ->
            if idInput <> "" then
                let deletedstudents = removeStudent id students
                saveStudents deletedstudents
                MessageBox.Show("Student deleted successfully!") |> ignore
        | _ -> 
        MessageBox.Show("Invalid ID! Please enter a numeric value.") |> ignore
    )
    form.Controls.Add(deleteButton)

    form


let createViewerForm (userId: int) =
    let form = new Form(Text = "Welcome To Students Grades Application", ClientSize = System.Drawing.Size(400, 350))
    let students = loadStudents()
    let student = students |> List.find (fun s -> s.ID = userId)

    // Welcome message with student name
    let welcomeLabel = new Label(
        Text = sprintf "Welcome %s!" student.Name,
        Location = System.Drawing.Point(50, 20),
        Size = System.Drawing.Size(300, 20)
    )
    form.Controls.Add(welcomeLabel)

    let gradesButton = new Button(
        Text = "Your Grades",
        Location = System.Drawing.Point(50, 50),
        Size = System.Drawing.Size(120, 30)
    )
    gradesButton.Click.Add(fun _ ->
        // Define the subject names in order
        let subjects = ["Math"; "Science"; "Arabic"; "English"]
    
        // Pair each subject with the corresponding grade
        let gradesWithSubjects =
            List.zip subjects student.Grades
            |> List.map (fun (subject, grade) -> sprintf "%s: %d" subject grade)
            |> String.concat "\n"

        // Show the grades in a formatted message
        MessageBox.Show(sprintf "Your Grades:\n%s" gradesWithSubjects) |> ignore
    )
    form.Controls.Add(gradesButton)


    // Your Average Button
    let avgButton = new Button(Text = "Your Average", Location = System.Drawing.Point(50, 100), Size = System.Drawing.Size(120, 30))
    avgButton.Click.Add(fun _ -> 
        let avg = getStudentAverage userId students
        MessageBox.Show(sprintf "Your Average: %.2f" avg) |> ignore
    )
    form.Controls.Add(avgButton)

    // Class Average Button
    let classAvgButton = new Button(Text = "Class Average", Location = System.Drawing.Point(50, 150), Size = System.Drawing.Size(120, 30))
    classAvgButton.Click.Add(fun _ -> 
        let avg = calculateClassAverage students
        MessageBox.Show(sprintf "Class Average: %.2f" avg) |> ignore
    )
    form.Controls.Add(classAvgButton)

    // Highest and Lowest Grades button
    let HighestLowestButton = new Button(Text = "highest & lowest grades", Location = System.Drawing.Point(50, 200), Size = System.Drawing.Size(140, 30))
    HighestLowestButton.Click.Add(fun _ ->
        match findHighestAndLowestAverages students with
        | None -> 
            ignore (MessageBox.Show("No students or grades available to analyze."))
        | Some (highest, lowest) -> 
            let message = sprintf "Highest Average Grades: %.2f\nLowest Average Grades: %.2f" highest lowest
            ignore (MessageBox.Show(message))
    )
    form.Controls.Add(HighestLowestButton)
    // Class Statistics button
    let statsButton = new Button(Text = "Class Statistics", Location = System.Drawing.Point(50, 250), Size = System.Drawing.Size(140, 30))
    statsButton.Click.Add(fun _ ->
        let passThreshold = float 50
        let statsMessage = calculateClassStatistics passThreshold students
        ignore (MessageBox.Show(statsMessage))
    )
    form.Controls.Add(statsButton)

    form

let CreateForm() =
    let form = new Form(Text = "Login", ClientSize = System.Drawing.Size(300, 200))

    let idLabel = new Label(Text = "ID:", Location = System.Drawing.Point(20, 20), Size = System.Drawing.Size(50, 20))
    let idInput = new TextBox(Location = System.Drawing.Point(100, 20), Size = System.Drawing.Size(150, 20))
    form.Controls.Add(idLabel)
    form.Controls.Add(idInput)

    let passwordLabel = new Label(Text = "Password:", Location = System.Drawing.Point(20, 60), Size = System.Drawing.Size(80, 20))
    let passwordInput = new TextBox(Location = System.Drawing.Point(100, 60), Size = System.Drawing.Size(150, 20), PasswordChar = '*')
    form.Controls.Add(passwordLabel)
    form.Controls.Add(passwordInput)

    let loginButton = new Button(Text = "Login", Location = System.Drawing.Point(100, 100), Size = System.Drawing.Size(80, 30))
    loginButton.Click.Add(fun _ -> 
        let idText = idInput.Text
        let password = passwordInput.Text

        if idText = "" then
            MessageBox.Show("ID cannot be empty!") |> ignore
        elif password = "" then
            MessageBox.Show("Password cannot be empty!") |> ignore
        else
            match System.Int32.TryParse(idText) with
            | (true, id) -> 
                match id, password with
                | 1, "ROLEadmin_1" -> 
                    let adminForm = createAdminForm()
                    adminForm.ShowDialog() |> ignore  // Show dialog modally
                | _ -> 
                    match loadStudents() |> List.tryFind (fun s -> s.ID = id && s.Password = password) with
                    | Some _ -> 
                        let studentForm = createViewerForm(id)
                        studentForm.ShowDialog() |> ignore  // Show dialog modally
                    | None -> 
                        MessageBox.Show("Invalid Credentials") |> ignore
            | _ -> 
                MessageBox.Show("ID must be a valid integer!") |> ignore
    )
    form.Controls.Add(loginButton)

    form