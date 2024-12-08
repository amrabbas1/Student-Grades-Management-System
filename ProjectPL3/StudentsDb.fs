module StudentsDb

open Newtonsoft.Json
open System.IO
open Models

let jsonFilePath = "students.json"

// Load students from a JSON file
let loadStudents() =
    if File.Exists(jsonFilePath) then
        JsonConvert.DeserializeObject<Student list>(File.ReadAllText(jsonFilePath))
    else
        []

// Save students to a JSON file
let saveStudents (students: Student list) =
    File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(students, Formatting.Indented))

// Add, edit, and remove students
let addStudent (student: Student) (db: Student list) = student :: db

let editStudent (id: int) (newStudent: Student) (db: Student list) =
    db |> List.map (fun s -> if s.ID = id then newStudent else s)

let removeStudent (id: int) (db: Student list) =
    db |> List.filter (fun s -> s.ID <> id)
