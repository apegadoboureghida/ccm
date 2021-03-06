using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Reflection;
namespace vsCCM
{
  /// <summary>The object for implementing an Add-in.</summary>
  /// <seealso class='IDTExtensibility2' />
  public class Connect : IDTExtensibility2, IDTCommandTarget
  {
    private Window m_toolWindow = null;

    /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
    public Connect()
    {
    }

    private void CreateToolWindow() 
    {
      object programmableObject = null;
      string guidString = "{73000449-0035-43ad-985F-A3071ECBC079}";
      Windows2 windows2 = (Windows2)_applicationObject.Windows;
      Assembly asm = Assembly.GetExecutingAssembly();
      m_toolWindow = windows2.CreateToolWindow2(_addInInstance, asm.Location,
          "vsCCM.ccmControl",
          "Code Complexity", guidString, ref programmableObject);
      m_toolWindow.Visible = true;

      ((ccmControl)m_toolWindow.Object).Initialize(this._applicationObject);
    }

    private void SetupToolsMenuAction()
    {
      try
      {
        object[] contextGUIDS = new object[] { };
        Commands2 commands = (Commands2)_applicationObject.Commands;
        string toolsMenuName = "Tools";

        Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];

        CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
        CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

        string name = "Cyclomatic Complexity";
        Command command =
          commands.AddNamedCommand2(_addInInstance, "ccm", name, name,
          true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

        command.AddControl(toolsPopup.CommandBar, 1);
      }
      catch (ArgumentException)
      {
      }

    }
    private void SetupSolutionExplorerContextMenus()
    {
      object[] contextGUIDS = new object[] { };
      Commands2 commands = (Commands2)_applicationObject.Commands;
      CommandBars cBars = (CommandBars)_applicationObject.CommandBars;
      try
      {
        string visibleCommandName = "Cyclomatic Complexity (ccm)";

        Command solutionCommand = commands.AddNamedCommand2(_addInInstance,
          "ccmRunSolution", visibleCommandName,
          "ccmRunSolution", false, 1, ref contextGUIDS,
          (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, 
          (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

        Command projectCommand = commands.AddNamedCommand2(_addInInstance,
          "ccmRunProject", visibleCommandName,
          "ccmRunProject", false, 1, ref contextGUIDS,
          (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled,
          (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

        Command itemCommand = commands.AddNamedCommand2(_addInInstance,
          "ccmRunItem", visibleCommandName,
          "ccmRunItem", false, 1, ref contextGUIDS,
          (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled,
          (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

        Microsoft.VisualStudio.CommandBars.CommandBar vsBarProject = cBars["Project"];
        Microsoft.VisualStudio.CommandBars.CommandBar vsBarSolution = cBars["Solution"];
        Microsoft.VisualStudio.CommandBars.CommandBar vsBarItem = cBars["Item"];

        solutionCommand.AddControl(vsBarSolution);
        projectCommand.AddControl(vsBarProject);
        itemCommand.AddControl(vsBarItem);
      }
      catch (System.ArgumentException)
      {
      }
    }

    /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
    /// <param term='application'>Root object of the host application.</param>
    /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
    /// <param term='addInInst'>Object representing this Add-in.</param>
    /// <seealso class='IDTExtensibility2' />
    public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
    {
      _applicationObject = (DTE2)application;
      _addInInstance = (AddIn)addInInst;

      try
      {
        CreateToolWindow();
        SetupSolutionExplorerContextMenus();
        SetupToolsMenuAction();
      }
      catch (Exception e)
      {
        System.Windows.Forms.MessageBox.Show(e.Message);
      }
    }

    /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
    /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
    /// <param term='custom'>Array of parameters that are host application specific.</param>
    /// <seealso class='IDTExtensibility2' />
    public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
    {
    }

    /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
    /// <param term='custom'>Array of parameters that are host application specific.</param>
    /// <seealso class='IDTExtensibility2' />		
    public void OnAddInsUpdate(ref Array custom)
    {
    }

    /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
    /// <param term='custom'>Array of parameters that are host application specific.</param>
    /// <seealso class='IDTExtensibility2' />
    public void OnStartupComplete(ref Array custom)
    {
    }

    /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
    /// <param term='custom'>Array of parameters that are host application specific.</param>
    /// <seealso class='IDTExtensibility2' />
    public void OnBeginShutdown(ref Array custom)
    {
    }
    
    private DTE2 _applicationObject;
    private AddIn _addInInstance;

    //#region IDTCommandTarget Members

    public void Exec(string CmdName, vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
    {
      if (null == m_toolWindow)
        CreateToolWindow();

      if (m_toolWindow != null)
        m_toolWindow.Activate();

      if (CmdName.Contains("ccmRunItem"))
      {
        ((ccmControl)m_toolWindow.Object).analyzeFile_Click(null, null);
      }
      else if (CmdName.Contains("ccmRunProject"))
      {
        ((ccmControl)m_toolWindow.Object).analyzeProject_Click(null, null);
      }
      else if (CmdName.Contains("ccmRunSolution"))
      {
        ((ccmControl)m_toolWindow.Object).analyzeSolution_Click(null, null);
      }
    }

    public void QueryStatus(string CmdName, vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
    {
      StatusOption = vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported;  
    }

    //#endregion
  }
}