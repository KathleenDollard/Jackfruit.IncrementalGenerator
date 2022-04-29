#r "nuget: System.CommandLine, 2.0.0-beta3.22114.1"

open System
open System.CommandLine
open System.Threading.Tasks

// boilerplate
let picard = Option<bool>("-picard")
let greeting = Argument<string>("<greeting>")

let nextGen =
    let c = Command("nextGeneration")
    c.Add(picard)
    c.Add(greeting)
    c

nextGen.AddCommand(nextGen)
nextGen.SetHandler(
    Func<_,_,Task>(fun (picard: bool) (greeting: string) -> 
        if picard then
            printfn $"Welcome to the next generation, Mr. {greeting}"
            Task.FromResult 0
        else
            printfn $"Welcome, {greeting}"
            Task.FromResult 0
    ), picard, greeting)

// validation
// the long way
nextGen.AddValidator(fun r ->
    // boilerplate
    let picard = r.GetValueForOption(picard)
    let greeting = r.GetValueForArgument(greeting)
    if picard && not (greeting.Contains ' ') 
    then
        // had to set this string myself
        r.ErrorMessage <- "Mr. Picard requires full names for his guests, please provide a full name for yourself."
    else
        ()
)

// ideal way - I just focus on my validation logic and the boilerplate of parameter resolution and error message propogation
// is handled for me
let validateFullName (picard: bool) (greetingArg: string): string = 
    if picard && not (greetingArg.Contains ' ') 
    then
        "Mr. Picard requires full names for his guests, please provide a full name for yourself."
    else
        null

let validatePoliteness (greetingArg: string) = 
    if greetingArg.Contains "please"
    then
        null
    else
        "We are polite on this ship, you hooligan!"

type Command with
    member x.AddValidator(d: Delegate) = () // you'd actually generate things here

// now, users can add their own validation delegate
nextGen.AddValidator (System.Func<_,_,_> validateFullName)
nextGen.AddValidator (System.Func<_,_> validatePoliteness)

// boilerplate that the generator could do. this fits the signature of Command.AddValidator
let validateGenerated (r: System.CommandLine.Parsing.CommandResult) =
    let picard = r.GetValueForOption(picard)
    let greetingArg = r.GetValueForArgument(greeting)
    // call the delegate
    let response1 = validateFullName picard greetingArg
    let response2 = validatePoliteness greetingArg
    // important to not directly add the responses to the error message without separating by newlines
    let responses = 
        [
            if response1 <> null then yield response1
            if response2 <> null then yield response2
        ]
        |> String.concat Environment.NewLine

    r.ErrorMessage <- responses
    // NOTE: in the case of multiple delegates, you may need to do some bookkeeping on the ErrorMessage property itself, making sure that newlines between entries are written.
    // NOTE: for even more perf, instead of extracting out the parameters for each validator separately, 

nextGen.AddValidator validateGenerated

nextGen.Invoke([| "nextGeneration"; "-picard"; "Chet Husk, please" |]) // no errors
nextGen.Invoke([| "nextGeneration"; "-picard"; "welp" |]) // two validation errors, one from each function we set up