namespace bomLite

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
    Jobs: ObservableCollection<string>
    ItemMasters: ObservableCollection<ItemMaster>
    BillOfMaterialsItems: ObservableCollection<JobOrderRow>

  }

module Main =
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
          Jobs = jobs
          
          BillOfMaterialsItems = connection.getJobOrderRows(firstJob) |> ObservableCollection
          ItemMasters = connection.getItemMasters() |> ObservableCollection} 
        let data = state |> ctx.useState 

        DockPanel.create [
          DockPanel.children [

            TextBlock.create [ 
              TextBlock.dock Dock.Top
              TextBlock.text ( sprintf "JobCurrent : %s" firstJob ) 
            ]
            ListBox.create[
              ListBox.dock Dock.Left
              ListBox.dataItems data.Current.Jobs

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
