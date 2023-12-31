﻿namespace bomLite

open System
open connection
open System.Collections.ObjectModel
open Avalonia
open Avalonia.Data
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.LiveView
open Avalonia.FuncUI.Experimental
open Avalonia.FuncUI.LiveView.Core.Types

module String=
  let decimalTryParse (s:string) =
    let mutable dec = 0m
    match Decimal.TryParse(s, ref dec) with
    | true -> Ok dec 
    | false -> Error 0m


  let extractDecimal (s:string) :decimal=
    match s.Length>0 with
    |false -> 0.0m 
    |true -> 
      let chars = s.ToCharArray()
      let digitsOrDecimal= 
        chars 
        |> Array.filter (fun c -> Char.IsDigit c || c = '.')
        |> string 
      match digitsOrDecimal |> decimalTryParse  with
      |Error e -> e
      |Ok dec -> dec
      
    // |> String

  let extractFloat (s:string) =
    match s.Length>0 with
    |false -> 0.0 
    |true -> 
      float(s |> extractDecimal)



type peeps = { Name: string; Age: int }

type myState =
  {
    CurrentJobSelection: JobOrderMaster
    Jobs: ObservableCollection<JobOrderMaster>
    ItemMasters: ObservableCollection<ItemMaster>
    BillOfMaterialsItems: ObservableCollection<JobOrderRow>

  }


[<RequireQualifiedAccess>]
type EditJobRowResult =
    | Cancel
    | Update of JobOrderRow

module Main =
  open Avalonia.Interactivity
  // let sqlData =
  //   conn
  [<AbstractClass; Sealed >]
  type Views =
    // let view () =
    // [<LivePreview>]
    static member view () =  
      Component(fun ctx ->
        let jobs =  connection.getJobMasters() |> ObservableCollection
        let firstJob = jobs.[0] 
        let state = {
          CurrentJobSelection = firstJob
          Jobs = jobs 
          BillOfMaterialsItems = connection.getJobOrderRows(firstJob.JobNumber) |> ObservableCollection
          ItemMasters = connection.getItemMasters() |> ObservableCollection} 
        let data = state |> ctx.useState 

        DockPanel.create [
          DockPanel.children [

            TextBlock.create [ 
              TextBlock.dock Dock.Top
              TextBlock.text ( sprintf "JobCurrent : %s" data.Current.CurrentJobSelection.JobNumber ) 
            ]
            ListBox.create[
              ListBox.dock Dock.Left
              ListBox.dataItems data.Current.Jobs
              ListBox.itemTemplate (DataTemplateView<_>.create (fun (data: JobOrderMaster) ->
                TextBlock.create [ TextBlock.text data.JobNumber ]))
              ListBox.onSelectionChanged (fun  s ->
                let lb :ListBox= s.Source :?> ListBox
                data.Set ( {
                    data.Current with 
                      CurrentJobSelection = ( lb.SelectedItem :?> JobOrderMaster  )
                      BillOfMaterialsItems = ( lb.SelectedItem :?> JobOrderMaster  ).JobNumber |> connection.getJobOrderRows |> ObservableCollection
                          }))

              // ListBox.width 75
            ]
            DataGrid.create [
              DataGrid.dock Dock.Right
              DataGrid.isReadOnly false
              DataGrid.items data.Current.BillOfMaterialsItems

              DataGrid.columns [
                DataGridTextColumn.create [
                  DataGridTextColumn.header "ItemNumber"
                  DataGridTextColumn.binding (Binding("ItemNumber", BindingMode.TwoWay))
                // DataGridTextColumn.width (DataGridLength(2, DataGridLengthUnitType.Star))
                ]
                DataGridTemplateColumn.create [
                  DataGridTemplateColumn.header "PartNumber"
                  DataGridTemplateColumn.cellTemplate (
                    DataTemplateView<_>.create (fun (data: JobOrderRow) ->
                      TextBlock.create [ TextBlock.text data.PartNumber ])
                  )
                  DataGridTemplateColumn.cellEditingTemplate (
                    DataTemplateView<_>.create (fun (data: JobOrderRow) ->
                      TextBox.create [
                        TextBox.init (fun t ->
                          t.Bind(TextBox.TextProperty, Binding("PartNumber", BindingMode.TwoWay))
                          |> ignore)
                      ])
                  )
                ]
                DataGridTemplateColumn.create [
                  DataGridTemplateColumn.header "Description"
                  DataGridTemplateColumn.cellTemplate (
                    DataTemplateView<_>.create (fun (data: JobOrderRow) ->
                      TextBlock.create [ TextBlock.text data.Description ])
                  )
                  DataGridTemplateColumn.cellEditingTemplate (
                    DataTemplateView<_>.create (fun (data: JobOrderRow) ->
                      TextBox.create [
                        TextBox.init (fun t ->
                          t.Bind(TextBox.TextProperty, Binding("Description", BindingMode.TwoWay))
                          |> ignore)
                      ])
                  )
                ]
                DataGridTemplateColumn.create [
                  DataGridTemplateColumn.header "Rev"
                  DataGridTemplateColumn.cellTemplate (
                    DataTemplateView<_>.create (fun (data: JobOrderRow) ->
                      TextBlock.create [ TextBlock.text data.RevisionNumber ])
                  )
                  DataGridTemplateColumn.cellEditingTemplate (
                    DataTemplateView<_>.create (fun (data: JobOrderRow) ->
                      TextBox.create [
                        TextBox.init (fun t ->
                          t.Bind(TextBox.TextProperty, Binding("RevisionNumber", BindingMode.TwoWay))
                          |> ignore)
                        TextBox.onTextInput (fun s -> 
                          let newText=
                            match s.Text.Length with
                              |l when l<1  -> "000"
                              |l when l>3 -> s.Text.Substring(0,3)
                              |l -> s.Text.PadLeft(3)
                          s.Text <- newText

                          data.RevisionNumber<- newText   )
                      ])
                  )
                ]
                DataGridTemplateColumn.create [
                  DataGridTemplateColumn.header "Quantity"
                  DataGridTemplateColumn.cellTemplate (
                    DataTemplateView<_>.create (fun (data: JobOrderRow) ->
                      TextBlock.create [ TextBlock.text (data.Quantity |> string)])
                  )
                  DataGridTemplateColumn.cellEditingTemplate (
                    DataTemplateView<_>.create (fun (data: JobOrderRow) ->
                      TextBox.create [
                        TextBox.init (fun t ->
                          t.AddHandler(
                            TextBox.TextInputEvent,
                            (fun sender args -> args.Text <- String.filter (fun c -> Char.IsDigit c || c = '.') args.Text),
                            RoutingStrategies.Tunnel)
                          t.Bind(TextBox.TextProperty, Binding("Quantity", BindingMode.TwoWay))
                          |> ignore)
                      ])
                  )
                ]
                // DataGridTextColumn.create [ DataGridTextColumn.header "Age"; DataGridTextColumn.binding (Binding "Age") ]
                // DataGridCheckBoxColumn.create [
                //   DataGridCheckBoxColumn.header "IsMale"
                //   DataGridCheckBoxColumn.binding (Binding "IsMale")
                // ]
              ]
            ]
          ]
        ])

type MainWindow() =
  inherit HostWindow()

  do
    base.Title <- "BOM Lite (Example project)"
    base.Width <- 700.0 
    base.Height <- 500.0 
    base.Content <- Main.Views.view ()

module LiveView =
  let enabled =
    match System.Environment.GetEnvironmentVariable("FUNCUI_LIVEPREVIEW") with
    // | null -> false
    // | "1" -> true
    | _ -> false

type App() = 
  inherit Application()
  override this.Initialize() =
    this.Styles.Add(FluentTheme())
    this.RequestedThemeVariant <- Styling.ThemeVariant.Dark
    this.Styles.Load "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml"

  override this.OnFrameworkInitializationCompleted() =
    match this.ApplicationLifetime with
    | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
      desktopLifetime.MainWindow <-
        if LiveView.enabled then
          LiveViewWindow() :> Window
        else
          MainWindow()
    | _ -> ()

#if DEBUG
    this.AttachDevTools()
#endif

module Program =

  [<EntryPoint>]
  let main (args: string[]) =
    // let ims = getItemMasters ()

    AppBuilder
      .Configure<App>()
      .UsePlatformDetect()
      .UseSkia()
      .StartWithClassicDesktopLifetime(args)
