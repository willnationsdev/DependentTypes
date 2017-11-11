﻿namespace DependentTypes.Tests

open robkuz.DependentTypes
open Expecto
open FsCheck

module DependentType =

    let config10k = { FsCheckConfig.defaultConfig with maxTest = 10000 }
    let configReplay = { FsCheckConfig.defaultConfig with maxTest = 10000 ; replay = Some <| (1940624926, 296296394) } // ; arbitrary = [typeof<DomainGenerators>] }  //see Tips & Tricks for FsCheck

    let validate normalize fn v =
        if fn (normalize v) then Some (normalize v) else None

    let validateLen len s = 
        validate id (fun (s:string) -> s.Length <= len) s

    type LenValidator(config) = 
        inherit Cctor<int, string, string>(config, validateLen)

    type Size5 () = inherit LenValidator(5) 

    type String5 = DependentType<Size5, int, string, string>

    [<Tests>]
    let dependentTypes =
        testList "DependentTypes.Equality and Comparison" [

            testCase "Equality" <| fun () ->
                let s100_1 =  (String5.TryParse "100").Value
                let s100_2 =  (String5.TryParse "100").Value

                Expect.equal s100_1 s100_2 "Expected equal"

            testCase "Inequality" <| fun () ->
                let s100 =  (String5.TryParse "100").Value
                let s200 =  (String5.TryParse "200").Value

                Expect.notEqual s100 s200 "Expected not equal"

            testCase "Comparison" <| fun () ->
                let n1 =  (String5.TryParse "100").Value
                let n2 =  (String5.TryParse "200").Value
                let n3 =  (String5.TryParse "300").Value
                let n4 =  (String5.TryParse "400").Value
                let n5 =  (String5.TryParse "500").Value

                let l1 = [n1; n2; n3; n4; n5]
                let l2 = [n5; n4; n1; n2; n3]

                Expect.equal l1 (l2 |> List.sort) "Expected equal"
        ]