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
