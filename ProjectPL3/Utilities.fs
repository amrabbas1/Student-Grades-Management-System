module Utilities
open Models

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
