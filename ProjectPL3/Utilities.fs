module Utilities

open Models
open System.Windows.Forms

// Input dialog for text input
let createInputDialog (prompt: string) (defaultValue: string) =
    let form = new Form()
    form.Text <- "Input"
    form.ClientSize <- System.Drawing.Size(300, 150)
    
    let label = new Label()
    label.Text <- prompt
    label.Location <- System.Drawing.Point(10, 10)
    label.Size <- System.Drawing.Size(300, 40)
    form.Controls.Add(label)

    let textBox = new TextBox()
    textBox.Text <- defaultValue
    textBox.Location <- System.Drawing.Point(10, 50)
    form.Controls.Add(textBox)

    let button = new Button()
    button.Text <- "OK"
    button.Location <- System.Drawing.Point(10, 80)
    button.DialogResult <- DialogResult.OK
    form.Controls.Add(button)
    
    form.AcceptButton <- button
    form.StartPosition <- FormStartPosition.CenterScreen

    if form.ShowDialog() = DialogResult.OK then
        textBox.Text
    else
        ""


let calculateClassStatistics (passThreshold: float) (db: Student list) =
    match db with
    | [] -> 
        "No students in the database."
    | _ -> 
        // Calculate statistics for each student
        let stats = 
            db |> List.map (fun student ->
                let avg = 
                    match student.Grades with
                    | [] -> 0.0  // No grades available
                    | _ -> 
                        (float (List.sum student.Grades)) / (float (List.length student.Grades))
                (student, avg >= passThreshold))
        
        // Total students
        let totalStudents = List.length db

        // Count passed and failed
        let passed, failed =
            stats |> List.fold (fun (p, f) (_, isPass) ->
                if isPass then (p + 1, f) else (p, f + 1)) (0, 0)
        
        // Calculate pass and fail rates
        let passRate = (float passed / float totalStudents) * 100.0
        let failRate = (float failed / float totalStudents) * 100.0

        // Create the result string
        sprintf "Class Statistics:\nTotal Students: %d\nPassed: %d (%.2f%%)\nFailed: %d (%.2f%%)" 
                totalStudents passed passRate failed failRate


// Calculate the average grade for a student
let getStudentAverage (id: int) (db: Student list) =
    match List.tryFind (fun s -> s.ID = id) db with
    | Some student when List.isEmpty student.Grades -> 0.0  // Return 0.0 if no grades
    | Some student -> 
        float (List.sum student.Grades) / float (List.length student.Grades)
    | None -> 
        -1.0  // Return -1.0 to indicate student not found


let calculateClassAverage students =
    let allGrades = students |> List.collect (fun s -> s.Grades)
    let total = List.sum allGrades
    let count = List.length allGrades
    float total / float count


let findHighestAndLowestAverages (db: Student list) =
    match db with
    | [] -> 
        None  // Return None if the database is empty
    | _ ->
        // Compute averages for each student
        let averages =
            db
            |> List.map (fun student -> 
                if student.Grades = [] then 0.0 // Handle empty grade lists
                else float (List.sum student.Grades) / float (List.length student.Grades))
        
        match averages with
        | [] -> 
            None  // No averages available
        | _ ->
            let highest = List.max averages
            let lowest = List.min averages
            Some (highest, lowest)  // Return a tuple with highest and lowest averages