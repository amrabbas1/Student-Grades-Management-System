﻿module UI

open System
open System.Windows.Forms
open System.Drawing
open Models
open Utilities
open StudentsDb

// Create a Windows Form to display students
let CreateForm () =
    let form = new Form(Text = "Student Grades Management System", ClientSize = System.Drawing.Size(400, 400))
    let mutable students = loadStudents()

    let addButton = new Button(Text = "Add Student", Location = System.Drawing.Point(50, 50), Size = System.Drawing.Size(100, 30))
    addButton.Click.Add(fun _ -> 
        let idInput = createInputDialog "Enter ID:" ""
        let nameInput = createInputDialog "Enter Name:" ""
        let gradesInput = createInputDialog "Enter Grades (comma-separated):" ""
        let passwordInput = createInputDialog "Enter Student Password:" ""  // Password input
        if idInput <> "" && nameInput <> "" && gradesInput <> "" then
            let id = int idInput
            let name = nameInput
            let grades = gradesInput.Split(',') |> Array.map int |> Array.toList
            let password = passwordInput
            let newStudent = { ID = id; Name = name; Grades = grades; Password = password }
            students <- addStudent newStudent students
            saveStudents students
            MessageBox.Show("Student added successfully!") |> ignore
    )
    form.Controls.Add(addButton)

    // Show Students
    let showButton = new Button(Text = "Show Students", Location = System.Drawing.Point(50, 100), Size = System.Drawing.Size(100, 30))
    showButton.Click.Add(fun _ -> 
        let studentList = students |> List.map (fun s -> sprintf "ID: %d, Name: %s, Grades: %A" s.ID s.Name s.Grades)
        MessageBox.Show(String.Join("\n", studentList)) |> ignore
    )
    form.Controls.Add(showButton)

    // Update Student
    let updateButton = new Button(Text = "Update Student", Location = System.Drawing.Point(50, 150), Size = System.Drawing.Size(100, 30))
    updateButton.Click.Add(fun _ -> 
        let idInput = createInputDialog "Enter Student ID to update:" ""
        let studentOpt = students |> List.tryFind (fun s -> s.ID = int idInput)
        match studentOpt with
        | Some student -> 
            let newName = createInputDialog "Enter new Name:" student.Name
            let newGrades = createInputDialog "Enter new Grades (comma-separated):" (String.Join(",", student.Grades))
            let updatedStudent = { student with Name = newName; Grades = newGrades.Split(',') |> Array.map int |> Array.toList }
            students <- editStudent student.ID updatedStudent students  // Corrected to use the new editStudent function
            saveStudents students
            MessageBox.Show("Student updated successfully!") |> ignore
        | None -> MessageBox.Show("Student not found.") |> ignore
    )
    form.Controls.Add(updateButton)

    // Delete Student
    let deleteButton = new Button(Text = "Delete Student", Location = System.Drawing.Point(50, 200), Size = System.Drawing.Size(100, 30))
    deleteButton.Click.Add(fun _ -> 
        let idInput = createInputDialog "Enter Student ID to delete:" ""
        students <- List.filter (fun s -> s.ID <> int idInput) students
        saveStudents students
        MessageBox.Show("Student deleted successfully!") |> ignore
    )
    form.Controls.Add(deleteButton)

    // Class Statistics button
    let statsButton = new Button(Text = "Class Statistics", Location = System.Drawing.Point(50, 250), Size = System.Drawing.Size(140, 30))
    statsButton.Click.Add(fun _ ->
        let thresholdInput = createInputDialog "Enter Pass Threshold:" "50"
        if thresholdInput <> "" then
            let passThreshold = float thresholdInput
            let statsMessage = calculateClassStatistics passThreshold students
            ignore (MessageBox.Show(statsMessage))
    )
    form.Controls.Add(statsButton)

    // Your Average Button
    let avgButton = new Button(Text = "Your Average", Location = System.Drawing.Point(50, 300), Size = System.Drawing.Size(120, 30))
    avgButton.Click.Add(fun _ ->
        // Prompt the user to enter their ID
        let idInput = createInputDialog "Enter Your ID:" ""
    
        if idInput <> "" then
            let enteredId = int idInput
            // Find the student by ID
            match students |> List.tryFind (fun s -> s.ID = enteredId) with
            | Some student -> 
                let avg = getStudentAverage enteredId students
                MessageBox.Show(sprintf "Your Average: %.2f" avg) |> ignore
            | None -> 
                MessageBox.Show("Invalid ID. Student not found.") |> ignore
        else
            MessageBox.Show("You must enter a valid ID.") |> ignore
    )
    form.Controls.Add(avgButton)


    // Class Average Button
    let classAvgButton = new Button(Text = "Class Average", Location = System.Drawing.Point(50, 350), Size = System.Drawing.Size(120, 30))
    classAvgButton.Click.Add(fun _ -> 
        let avg = calculateClassAverage students
        MessageBox.Show(sprintf "Class Average: %.2f" avg) |> ignore
    )
    form.Controls.Add(classAvgButton)

    // Highest and Lowest Grades button
    let avgGradesButton = new Button(Text = "highest & lowest grades", Location = System.Drawing.Point(170, 50), Size = System.Drawing.Size(140, 30))
    avgGradesButton.Click.Add(fun _ ->
        match findHighestAndLowestAverages students with
        | None -> 
            ignore (MessageBox.Show("No students or grades available to analyze."))
        | Some (highest, lowest) -> 
            let message = sprintf "Highest Average Grade: %.2f\nLowest Average Grade: %.2f" highest lowest
            ignore (MessageBox.Show(message))
    )
    form.Controls.Add(avgGradesButton)

    form