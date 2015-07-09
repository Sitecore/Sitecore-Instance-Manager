namespace SIM.Pipelines.SitecoreWebservices
{
  using System.ComponentModel;
  using System.Runtime.Serialization;
  using System.ServiceModel;
  using System.ServiceModel.Channels;
  using System.Xml.Linq;

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
  [System.Runtime.Serialization.DataContractAttribute(Name = "Credentials", Namespace = "http://sitecore.net/visual/")]
  [System.SerializableAttribute()]
  public partial class Credentials : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
  {
    #region Fields

    [System.Runtime.Serialization.OptionalFieldAttribute()]
    private string CustomDataField;

    [System.Runtime.Serialization.OptionalFieldAttribute()]
    private string PasswordField;

    [System.Runtime.Serialization.OptionalFieldAttribute()]
    private string UserNameField;

    [System.NonSerializedAttribute()]
    private ExtensionDataObject extensionDataField;

    #endregion

    #region Public properties

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false)]
    public string CustomData
    {
      get
      {
        return this.CustomDataField;
      }

      set
      {
        if (object.ReferenceEquals(this.CustomDataField, value) != true)
        {
          this.CustomDataField = value;
          this.RaisePropertyChanged("CustomData");
        }
      }
    }

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false)]
    public string Password
    {
      get
      {
        return this.PasswordField;
      }

      set
      {
        if (object.ReferenceEquals(this.PasswordField, value) != true)
        {
          this.PasswordField = value;
          this.RaisePropertyChanged("Password");
        }
      }
    }

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false)]
    public string UserName
    {
      get
      {
        return this.UserNameField;
      }

      set
      {
        if (object.ReferenceEquals(this.UserNameField, value) != true)
        {
          this.UserNameField = value;
          this.RaisePropertyChanged("UserName");
        }
      }
    }

    #endregion

    #region IExtensibleDataObject Members

    [global::System.ComponentModel.BrowsableAttribute(false)]
    public ExtensionDataObject ExtensionData
    {
      get
      {
        return this.extensionDataField;
      }

      set
      {
        this.extensionDataField = value;
      }
    }

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Protected methods

    protected void RaisePropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged != null)
      {
        propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ServiceModel.ServiceContractAttribute(Name = "Visual Sitecore ServiceSoap", Namespace = "http://sitecore.net/visual/", ConfigurationName = "SitecoreWebservices.VisualSitecoreServiceSoap")]
  public interface VisualSitecoreServiceSoap
  {
    // CODEGEN: Generating message contract since element name id from namespace http://sitecore.net/visual/ is not marked nillable
    #region Public methods

    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/AddFromMaster", ReplyAction = "*")]
    AddFromMasterResponse AddFromMaster(AddFromMasterRequest request);

    // CODEGEN: Generating message contract since element name id from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/AddFromTemplate", ReplyAction = "*")]
    AddFromTemplateResponse AddFromTemplate(AddFromTemplateRequest request);

    // CODEGEN: Generating message contract since element name id from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/AddVersion", ReplyAction = "*")]
    AddVersionResponse AddVersion(AddVersionRequest request);

    // CODEGEN: Generating message contract since element name id from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/CopyTo", ReplyAction = "*")]
    CopyToResponse CopyTo(CopyToRequest request);

    // CODEGEN: Generating message contract since element name id from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/Delete", ReplyAction = "*")]
    DeleteResponse Delete(DeleteRequest request);

    // CODEGEN: Generating message contract since element name id from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/DeleteChildren", ReplyAction = "*")]
    DeleteChildrenResponse DeleteChildren(DeleteChildrenRequest request);

    // CODEGEN: Generating message contract since element name id from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/Duplicate", ReplyAction = "*")]
    DuplicateResponse Duplicate(DuplicateRequest request);

    // CODEGEN: Generating message contract since element name id from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/GetChildren", ReplyAction = "*")]
    GetChildrenResponse GetChildren(GetChildrenRequest request);

    // CODEGEN: Generating message contract since element name credentials from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/GetDatabases", ReplyAction = "*")]
    GetDatabasesResponse GetDatabases(GetDatabasesRequest request);

    // CODEGEN: Generating message contract since element name id from namespace http://sitecore.net/visual/ is not marked nillable

    // CODEGEN: Generating message contract since element name id from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/GetItemFields", ReplyAction = "*")]
    GetItemFieldsResponse GetItemFields(GetItemFieldsRequest request);

    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/GetItemMasters", ReplyAction = "*")]
    GetItemMastersResponse GetItemMasters(GetItemMastersRequest request);

    // CODEGEN: Generating message contract since element name databaseName from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/GetLanguages", ReplyAction = "*")]
    GetLanguagesResponse GetLanguages(GetLanguagesRequest request);

    // CODEGEN: Generating message contract since element name databaseName from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/GetMasters", ReplyAction = "*")]
    GetMastersResponse GetMasters(GetMastersRequest request);

    // CODEGEN: Generating message contract since element name databaseName from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/GetTemplates", ReplyAction = "*")]
    GetTemplatesResponse GetTemplates(GetTemplatesRequest request);

    // CODEGEN: Generating message contract since element name id from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/GetXML", ReplyAction = "*")]
    GetXMLResponse GetXML(GetXMLRequest request);

    // CODEGEN: Generating message contract since element name id from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/InsertXML", ReplyAction = "*")]
    InsertXMLResponse InsertXML(InsertXMLRequest request);

    // CODEGEN: Generating message contract since element name id from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/MoveTo", ReplyAction = "*")]
    MoveToResponse MoveTo(MoveToRequest request);

    // CODEGEN: Generating message contract since element name id from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/RemoveVersion", ReplyAction = "*")]
    RemoveVersionResponse RemoveVersion(RemoveVersionRequest request);

    // CODEGEN: Generating message contract since element name id from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/Rename", ReplyAction = "*")]
    RenameResponse Rename(RenameRequest request);

    // CODEGEN: Generating message contract since element name xml from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/Save", ReplyAction = "*")]
    SaveResponse Save(SaveRequest request);

    // CODEGEN: Generating message contract since element name credentials from namespace http://sitecore.net/visual/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action = "http://sitecore.net/visual/VerifyCredentials", ReplyAction = "*")]
    VerifyCredentialsResponse VerifyCredentials(VerifyCredentialsRequest request);

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class AddFromMasterRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "AddFromMaster", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public AddFromMasterRequestBody Body;

    #endregion

    #region Constructors

    public AddFromMasterRequest()
    {
    }

    public AddFromMasterRequest(AddFromMasterRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class AddFromMasterRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 4)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string id;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public string masterID;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
    public string name;

    #endregion

    #region Constructors

    public AddFromMasterRequestBody()
    {
    }

    public AddFromMasterRequestBody(string id, string masterID, string name, string databaseName, Credentials credentials)
    {
      this.id = id;
      this.masterID = masterID;
      this.name = name;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class AddFromMasterResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "AddFromMasterResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public AddFromMasterResponseBody Body;

    #endregion

    #region Constructors

    public AddFromMasterResponse()
    {
    }

    public AddFromMasterResponse(AddFromMasterResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class AddFromMasterResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement AddFromMasterResult;

    #endregion

    #region Constructors

    public AddFromMasterResponseBody()
    {
    }

    public AddFromMasterResponseBody(XElement AddFromMasterResult)
    {
      this.AddFromMasterResult = AddFromMasterResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class AddFromTemplateRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "AddFromTemplate", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public AddFromTemplateRequestBody Body;

    #endregion

    #region Constructors

    public AddFromTemplateRequest()
    {
    }

    public AddFromTemplateRequest(AddFromTemplateRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class AddFromTemplateRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 4)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string id;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
    public string name;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public string templateID;

    #endregion

    #region Constructors

    public AddFromTemplateRequestBody()
    {
    }

    public AddFromTemplateRequestBody(string id, string templateID, string name, string databaseName, Credentials credentials)
    {
      this.id = id;
      this.templateID = templateID;
      this.name = name;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class AddFromTemplateResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "AddFromTemplateResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public AddFromTemplateResponseBody Body;

    #endregion

    #region Constructors

    public AddFromTemplateResponse()
    {
    }

    public AddFromTemplateResponse(AddFromTemplateResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class AddFromTemplateResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement AddFromTemplateResult;

    #endregion

    #region Constructors

    public AddFromTemplateResponseBody()
    {
    }

    public AddFromTemplateResponseBody(XElement AddFromTemplateResult)
    {
      this.AddFromTemplateResult = AddFromTemplateResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class AddVersionRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "AddVersion", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public AddVersionRequestBody Body;

    #endregion

    #region Constructors

    public AddVersionRequest()
    {
    }

    public AddVersionRequest(AddVersionRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class AddVersionRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string id;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public string language;

    #endregion

    #region Constructors

    public AddVersionRequestBody()
    {
    }

    public AddVersionRequestBody(string id, string language, string databaseName, Credentials credentials)
    {
      this.id = id;
      this.language = language;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class AddVersionResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "AddVersionResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public AddVersionResponseBody Body;

    #endregion

    #region Constructors

    public AddVersionResponse()
    {
    }

    public AddVersionResponse(AddVersionResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class AddVersionResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement AddVersionResult;

    #endregion

    #region Constructors

    public AddVersionResponseBody()
    {
    }

    public AddVersionResponseBody(XElement AddVersionResult)
    {
      this.AddVersionResult = AddVersionResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class CopyToRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "CopyTo", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public CopyToRequestBody Body;

    #endregion

    #region Constructors

    public CopyToRequest()
    {
    }

    public CopyToRequest(CopyToRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class CopyToRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 4)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string id;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
    public string name;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public string newParent;

    #endregion

    #region Constructors

    public CopyToRequestBody()
    {
    }

    public CopyToRequestBody(string id, string newParent, string name, string databaseName, Credentials credentials)
    {
      this.id = id;
      this.newParent = newParent;
      this.name = name;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class CopyToResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "CopyToResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public CopyToResponseBody Body;

    #endregion

    #region Constructors

    public CopyToResponse()
    {
    }

    public CopyToResponse(CopyToResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class CopyToResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement CopyToResult;

    #endregion

    #region Constructors

    public CopyToResponseBody()
    {
    }

    public CopyToResponseBody(XElement CopyToResult)
    {
      this.CopyToResult = CopyToResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class DeleteRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "Delete", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public DeleteRequestBody Body;

    #endregion

    #region Constructors

    public DeleteRequest()
    {
    }

    public DeleteRequest(DeleteRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class DeleteRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string id;

    [System.Runtime.Serialization.DataMemberAttribute(Order = 1)]
    public bool recycle;

    #endregion

    #region Constructors

    public DeleteRequestBody()
    {
    }

    public DeleteRequestBody(string id, bool recycle, string databaseName, Credentials credentials)
    {
      this.id = id;
      this.recycle = recycle;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class DeleteResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "DeleteResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public DeleteResponseBody Body;

    #endregion

    #region Constructors

    public DeleteResponse()
    {
    }

    public DeleteResponse(DeleteResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class DeleteResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement DeleteResult;

    #endregion

    #region Constructors

    public DeleteResponseBody()
    {
    }

    public DeleteResponseBody(XElement DeleteResult)
    {
      this.DeleteResult = DeleteResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class DeleteChildrenRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "DeleteChildren", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public DeleteChildrenRequestBody Body;

    #endregion

    #region Constructors

    public DeleteChildrenRequest()
    {
    }

    public DeleteChildrenRequest(DeleteChildrenRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class DeleteChildrenRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string id;

    #endregion

    #region Constructors

    public DeleteChildrenRequestBody()
    {
    }

    public DeleteChildrenRequestBody(string id, string databaseName, Credentials credentials)
    {
      this.id = id;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class DeleteChildrenResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "DeleteChildrenResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public DeleteChildrenResponseBody Body;

    #endregion

    #region Constructors

    public DeleteChildrenResponse()
    {
    }

    public DeleteChildrenResponse(DeleteChildrenResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class DeleteChildrenResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement DeleteChildrenResult;

    #endregion

    #region Constructors

    public DeleteChildrenResponseBody()
    {
    }

    public DeleteChildrenResponseBody(XElement DeleteChildrenResult)
    {
      this.DeleteChildrenResult = DeleteChildrenResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class DuplicateRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "Duplicate", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public DuplicateRequestBody Body;

    #endregion

    #region Constructors

    public DuplicateRequest()
    {
    }

    public DuplicateRequest(DuplicateRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class DuplicateRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string id;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public string name;

    #endregion

    #region Constructors

    public DuplicateRequestBody()
    {
    }

    public DuplicateRequestBody(string id, string name, string databaseName, Credentials credentials)
    {
      this.id = id;
      this.name = name;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class DuplicateResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "DuplicateResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public DuplicateResponseBody Body;

    #endregion

    #region Constructors

    public DuplicateResponse()
    {
    }

    public DuplicateResponse(DuplicateResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class DuplicateResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement DuplicateResult;

    #endregion

    #region Constructors

    public DuplicateResponseBody()
    {
    }

    public DuplicateResponseBody(XElement DuplicateResult)
    {
      this.DuplicateResult = DuplicateResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetChildrenRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetChildren", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetChildrenRequestBody Body;

    #endregion

    #region Constructors

    public GetChildrenRequest()
    {
    }

    public GetChildrenRequest(GetChildrenRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetChildrenRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string id;

    #endregion

    #region Constructors

    public GetChildrenRequestBody()
    {
    }

    public GetChildrenRequestBody(string id, string databaseName, Credentials credentials)
    {
      this.id = id;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetChildrenResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetChildrenResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetChildrenResponseBody Body;

    #endregion

    #region Constructors

    public GetChildrenResponse()
    {
    }

    public GetChildrenResponse(GetChildrenResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetChildrenResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement GetChildrenResult;

    #endregion

    #region Constructors

    public GetChildrenResponseBody()
    {
    }

    public GetChildrenResponseBody(XElement GetChildrenResult)
    {
      this.GetChildrenResult = GetChildrenResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetDatabasesRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetDatabases", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetDatabasesRequestBody Body;

    #endregion

    #region Constructors

    public GetDatabasesRequest()
    {
    }

    public GetDatabasesRequest(GetDatabasesRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetDatabasesRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public Credentials credentials;

    #endregion

    #region Constructors

    public GetDatabasesRequestBody()
    {
    }

    public GetDatabasesRequestBody(Credentials credentials)
    {
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetDatabasesResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetDatabasesResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetDatabasesResponseBody Body;

    #endregion

    #region Constructors

    public GetDatabasesResponse()
    {
    }

    public GetDatabasesResponse(GetDatabasesResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetDatabasesResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement GetDatabasesResult;

    #endregion

    #region Constructors

    public GetDatabasesResponseBody()
    {
    }

    public GetDatabasesResponseBody(XElement GetDatabasesResult)
    {
      this.GetDatabasesResult = GetDatabasesResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetItemMastersRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetItemMasters", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetItemMastersRequestBody Body;

    #endregion

    #region Constructors

    public GetItemMastersRequest()
    {
    }

    public GetItemMastersRequest(GetItemMastersRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetItemMastersRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string id;

    #endregion

    #region Constructors

    public GetItemMastersRequestBody()
    {
    }

    public GetItemMastersRequestBody(string id, string databaseName, Credentials credentials)
    {
      this.id = id;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetItemMastersResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetItemMastersResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetItemMastersResponseBody Body;

    #endregion

    #region Constructors

    public GetItemMastersResponse()
    {
    }

    public GetItemMastersResponse(GetItemMastersResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetItemMastersResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement GetItemMastersResult;

    #endregion

    #region Constructors

    public GetItemMastersResponseBody()
    {
    }

    public GetItemMastersResponseBody(XElement GetItemMastersResult)
    {
      this.GetItemMastersResult = GetItemMastersResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetItemFieldsRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetItemFields", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetItemFieldsRequestBody Body;

    #endregion

    #region Constructors

    public GetItemFieldsRequest()
    {
    }

    public GetItemFieldsRequest(GetItemFieldsRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetItemFieldsRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(Order = 3)]
    public bool allFields;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 4)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string id;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public string language;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
    public string version;

    #endregion

    #region Constructors

    public GetItemFieldsRequestBody()
    {
    }

    public GetItemFieldsRequestBody(string id, string language, string version, bool allFields, string databaseName, Credentials credentials)
    {
      this.id = id;
      this.language = language;
      this.version = version;
      this.allFields = allFields;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetItemFieldsResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetItemFieldsResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetItemFieldsResponseBody Body;

    #endregion

    #region Constructors

    public GetItemFieldsResponse()
    {
    }

    public GetItemFieldsResponse(GetItemFieldsResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetItemFieldsResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement GetItemFieldsResult;

    #endregion

    #region Constructors

    public GetItemFieldsResponseBody()
    {
    }

    public GetItemFieldsResponseBody(XElement GetItemFieldsResult)
    {
      this.GetItemFieldsResult = GetItemFieldsResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetLanguagesRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetLanguages", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetLanguagesRequestBody Body;

    #endregion

    #region Constructors

    public GetLanguagesRequest()
    {
    }

    public GetLanguagesRequest(GetLanguagesRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetLanguagesRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string databaseName;

    #endregion

    #region Constructors

    public GetLanguagesRequestBody()
    {
    }

    public GetLanguagesRequestBody(string databaseName, Credentials credentials)
    {
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetLanguagesResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetLanguagesResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetLanguagesResponseBody Body;

    #endregion

    #region Constructors

    public GetLanguagesResponse()
    {
    }

    public GetLanguagesResponse(GetLanguagesResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetLanguagesResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement GetLanguagesResult;

    #endregion

    #region Constructors

    public GetLanguagesResponseBody()
    {
    }

    public GetLanguagesResponseBody(XElement GetLanguagesResult)
    {
      this.GetLanguagesResult = GetLanguagesResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetMastersRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetMasters", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetMastersRequestBody Body;

    #endregion

    #region Constructors

    public GetMastersRequest()
    {
    }

    public GetMastersRequest(GetMastersRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetMastersRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string databaseName;

    #endregion

    #region Constructors

    public GetMastersRequestBody()
    {
    }

    public GetMastersRequestBody(string databaseName, Credentials credentials)
    {
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetMastersResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetMastersResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetMastersResponseBody Body;

    #endregion

    #region Constructors

    public GetMastersResponse()
    {
    }

    public GetMastersResponse(GetMastersResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetMastersResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement GetMastersResult;

    #endregion

    #region Constructors

    public GetMastersResponseBody()
    {
    }

    public GetMastersResponseBody(XElement GetMastersResult)
    {
      this.GetMastersResult = GetMastersResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetTemplatesRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetTemplates", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetTemplatesRequestBody Body;

    #endregion

    #region Constructors

    public GetTemplatesRequest()
    {
    }

    public GetTemplatesRequest(GetTemplatesRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetTemplatesRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string databaseName;

    #endregion

    #region Constructors

    public GetTemplatesRequestBody()
    {
    }

    public GetTemplatesRequestBody(string databaseName, Credentials credentials)
    {
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetTemplatesResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetTemplatesResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetTemplatesResponseBody Body;

    #endregion

    #region Constructors

    public GetTemplatesResponse()
    {
    }

    public GetTemplatesResponse(GetTemplatesResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetTemplatesResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement GetTemplatesResult;

    #endregion

    #region Constructors

    public GetTemplatesResponseBody()
    {
    }

    public GetTemplatesResponseBody(XElement GetTemplatesResult)
    {
      this.GetTemplatesResult = GetTemplatesResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetXMLRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetXML", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetXMLRequestBody Body;

    #endregion

    #region Constructors

    public GetXMLRequest()
    {
    }

    public GetXMLRequest(GetXMLRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetXMLRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(Order = 1)]
    public bool deep;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string id;

    #endregion

    #region Constructors

    public GetXMLRequestBody()
    {
    }

    public GetXMLRequestBody(string id, bool deep, string databaseName, Credentials credentials)
    {
      this.id = id;
      this.deep = deep;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class GetXMLResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "GetXMLResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public GetXMLResponseBody Body;

    #endregion

    #region Constructors

    public GetXMLResponse()
    {
    }

    public GetXMLResponse(GetXMLResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class GetXMLResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement GetXMLResult;

    #endregion

    #region Constructors

    public GetXMLResponseBody()
    {
    }

    public GetXMLResponseBody(XElement GetXMLResult)
    {
      this.GetXMLResult = GetXMLResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class InsertXMLRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "InsertXML", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public InsertXMLRequestBody Body;

    #endregion

    #region Constructors

    public InsertXMLRequest()
    {
    }

    public InsertXMLRequest(InsertXMLRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class InsertXMLRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
    public bool changeIDs;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 4)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string id;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public string xml;

    #endregion

    #region Constructors

    public InsertXMLRequestBody()
    {
    }

    public InsertXMLRequestBody(string id, string xml, bool changeIDs, string databaseName, Credentials credentials)
    {
      this.id = id;
      this.xml = xml;
      this.changeIDs = changeIDs;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class InsertXMLResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "InsertXMLResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public InsertXMLResponseBody Body;

    #endregion

    #region Constructors

    public InsertXMLResponse()
    {
    }

    public InsertXMLResponse(InsertXMLResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class InsertXMLResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement InsertXMLResult;

    #endregion

    #region Constructors

    public InsertXMLResponseBody()
    {
    }

    public InsertXMLResponseBody(XElement InsertXMLResult)
    {
      this.InsertXMLResult = InsertXMLResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class MoveToRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "MoveTo", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public MoveToRequestBody Body;

    #endregion

    #region Constructors

    public MoveToRequest()
    {
    }

    public MoveToRequest(MoveToRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class MoveToRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string id;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public string newParent;

    #endregion

    #region Constructors

    public MoveToRequestBody()
    {
    }

    public MoveToRequestBody(string id, string newParent, string databaseName, Credentials credentials)
    {
      this.id = id;
      this.newParent = newParent;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class MoveToResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "MoveToResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public MoveToResponseBody Body;

    #endregion

    #region Constructors

    public MoveToResponse()
    {
    }

    public MoveToResponse(MoveToResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class MoveToResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement MoveToResult;

    #endregion

    #region Constructors

    public MoveToResponseBody()
    {
    }

    public MoveToResponseBody(XElement MoveToResult)
    {
      this.MoveToResult = MoveToResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class RemoveVersionRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "RemoveVersion", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public RemoveVersionRequestBody Body;

    #endregion

    #region Constructors

    public RemoveVersionRequest()
    {
    }

    public RemoveVersionRequest(RemoveVersionRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class RemoveVersionRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 4)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string id;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public string language;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
    public string version;

    #endregion

    #region Constructors

    public RemoveVersionRequestBody()
    {
    }

    public RemoveVersionRequestBody(string id, string language, string version, string databaseName, Credentials credentials)
    {
      this.id = id;
      this.language = language;
      this.version = version;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class RemoveVersionResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "RemoveVersionResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public RemoveVersionResponseBody Body;

    #endregion

    #region Constructors

    public RemoveVersionResponse()
    {
    }

    public RemoveVersionResponse(RemoveVersionResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class RemoveVersionResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement RemoveVersionResult;

    #endregion

    #region Constructors

    public RemoveVersionResponseBody()
    {
    }

    public RemoveVersionResponseBody(XElement RemoveVersionResult)
    {
      this.RemoveVersionResult = RemoveVersionResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class RenameRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "Rename", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public RenameRequestBody Body;

    #endregion

    #region Constructors

    public RenameRequest()
    {
    }

    public RenameRequest(RenameRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class RenameRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string id;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public string newName;

    #endregion

    #region Constructors

    public RenameRequestBody()
    {
    }

    public RenameRequestBody(string id, string newName, string databaseName, Credentials credentials)
    {
      this.id = id;
      this.newName = newName;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class RenameResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "RenameResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public RenameResponseBody Body;

    #endregion

    #region Constructors

    public RenameResponse()
    {
    }

    public RenameResponse(RenameResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class RenameResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement RenameResult;

    #endregion

    #region Constructors

    public RenameResponseBody()
    {
    }

    public RenameResponseBody(XElement RenameResult)
    {
      this.RenameResult = RenameResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class SaveRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "Save", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public SaveRequestBody Body;

    #endregion

    #region Constructors

    public SaveRequest()
    {
    }

    public SaveRequest(SaveRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class SaveRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
    public Credentials credentials;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
    public string databaseName;

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public string xml;

    #endregion

    #region Constructors

    public SaveRequestBody()
    {
    }

    public SaveRequestBody(string xml, string databaseName, Credentials credentials)
    {
      this.xml = xml;
      this.databaseName = databaseName;
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class SaveResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "SaveResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public SaveResponseBody Body;

    #endregion

    #region Constructors

    public SaveResponse()
    {
    }

    public SaveResponse(SaveResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class SaveResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement SaveResult;

    #endregion

    #region Constructors

    public SaveResponseBody()
    {
    }

    public SaveResponseBody(XElement SaveResult)
    {
      this.SaveResult = SaveResult;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class VerifyCredentialsRequest
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "VerifyCredentials", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public VerifyCredentialsRequestBody Body;

    #endregion

    #region Constructors

    public VerifyCredentialsRequest()
    {
    }

    public VerifyCredentialsRequest(VerifyCredentialsRequestBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class VerifyCredentialsRequestBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public Credentials credentials;

    #endregion

    #region Constructors

    public VerifyCredentialsRequestBody()
    {
    }

    public VerifyCredentialsRequestBody(Credentials credentials)
    {
      this.credentials = credentials;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
  public partial class VerifyCredentialsResponse
  {
    #region Fields

    [System.ServiceModel.MessageBodyMemberAttribute(Name = "VerifyCredentialsResponse", Namespace = "http://sitecore.net/visual/", Order = 0)]
    public VerifyCredentialsResponseBody Body;

    #endregion

    #region Constructors

    public VerifyCredentialsResponse()
    {
    }

    public VerifyCredentialsResponse(VerifyCredentialsResponseBody Body)
    {
      this.Body = Body;
    }

    #endregion
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://sitecore.net/visual/")]
  public partial class VerifyCredentialsResponseBody
  {
    #region Fields

    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
    public XElement VerifyCredentialsResult;

    #endregion

    #region Constructors

    public VerifyCredentialsResponseBody()
    {
    }

    public VerifyCredentialsResponseBody(XElement VerifyCredentialsResult)
    {
      this.VerifyCredentialsResult = VerifyCredentialsResult;
    }

    #endregion
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  public interface VisualSitecoreServiceSoapChannel : SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap, System.ServiceModel.IClientChannel
  {
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
  public partial class VisualSitecoreServiceSoapClient : System.ServiceModel.ClientBase<VisualSitecoreServiceSoap>, SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap
  {
    #region Constructors

    public VisualSitecoreServiceSoapClient()
    {
    }

    public VisualSitecoreServiceSoapClient(string endpointConfigurationName) :
      base(endpointConfigurationName)
    {
    }

    public VisualSitecoreServiceSoapClient(string endpointConfigurationName, string remoteAddress) :
      base(endpointConfigurationName, remoteAddress)
    {
    }

    public VisualSitecoreServiceSoapClient(string endpointConfigurationName, EndpointAddress remoteAddress) :
      base(endpointConfigurationName, remoteAddress)
    {
    }

    public VisualSitecoreServiceSoapClient(Binding binding, EndpointAddress remoteAddress) :
      base(binding, remoteAddress)
    {
    }

    #endregion

    #region VisualSitecoreServiceSoap Members

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    AddFromMasterResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.AddFromMaster(AddFromMasterRequest request)
    {
      return base.Channel.AddFromMaster(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    AddFromTemplateResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.AddFromTemplate(AddFromTemplateRequest request)
    {
      return base.Channel.AddFromTemplate(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    AddVersionResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.AddVersion(AddVersionRequest request)
    {
      return base.Channel.AddVersion(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    CopyToResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.CopyTo(CopyToRequest request)
    {
      return base.Channel.CopyTo(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    DeleteResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.Delete(DeleteRequest request)
    {
      return base.Channel.Delete(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    DeleteChildrenResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.DeleteChildren(DeleteChildrenRequest request)
    {
      return base.Channel.DeleteChildren(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    DuplicateResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.Duplicate(DuplicateRequest request)
    {
      return base.Channel.Duplicate(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    GetChildrenResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.GetChildren(GetChildrenRequest request)
    {
      return base.Channel.GetChildren(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    GetDatabasesResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.GetDatabases(GetDatabasesRequest request)
    {
      return base.Channel.GetDatabases(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    GetItemFieldsResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.GetItemFields(GetItemFieldsRequest request)
    {
      return base.Channel.GetItemFields(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    GetItemMastersResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.GetItemMasters(GetItemMastersRequest request)
    {
      return base.Channel.GetItemMasters(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    GetLanguagesResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.GetLanguages(GetLanguagesRequest request)
    {
      return base.Channel.GetLanguages(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    GetMastersResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.GetMasters(GetMastersRequest request)
    {
      return base.Channel.GetMasters(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    GetTemplatesResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.GetTemplates(GetTemplatesRequest request)
    {
      return base.Channel.GetTemplates(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    GetXMLResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.GetXML(GetXMLRequest request)
    {
      return base.Channel.GetXML(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    InsertXMLResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.InsertXML(InsertXMLRequest request)
    {
      return base.Channel.InsertXML(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    MoveToResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.MoveTo(MoveToRequest request)
    {
      return base.Channel.MoveTo(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    RemoveVersionResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.RemoveVersion(RemoveVersionRequest request)
    {
      return base.Channel.RemoveVersion(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    RenameResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.Rename(RenameRequest request)
    {
      return base.Channel.Rename(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    SaveResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.Save(SaveRequest request)
    {
      return base.Channel.Save(request);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    VerifyCredentialsResponse SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap.VerifyCredentials(VerifyCredentialsRequest request)
    {
      return base.Channel.VerifyCredentials(request);
    }

    #endregion

    #region Public methods

    public XElement AddFromMaster(string id, string masterID, string name, string databaseName, Credentials credentials)
    {
      AddFromMasterRequest inValue = new SIM.Pipelines.SitecoreWebservices.AddFromMasterRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.AddFromMasterRequestBody();
      inValue.Body.id = id;
      inValue.Body.masterID = masterID;
      inValue.Body.name = name;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      AddFromMasterResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).AddFromMaster(inValue);
      return retVal.Body.AddFromMasterResult;
    }

    public XElement AddFromTemplate(string id, string templateID, string name, string databaseName, Credentials credentials)
    {
      AddFromTemplateRequest inValue = new SIM.Pipelines.SitecoreWebservices.AddFromTemplateRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.AddFromTemplateRequestBody();
      inValue.Body.id = id;
      inValue.Body.templateID = templateID;
      inValue.Body.name = name;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      AddFromTemplateResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).AddFromTemplate(inValue);
      return retVal.Body.AddFromTemplateResult;
    }

    public XElement AddVersion(string id, string language, string databaseName, Credentials credentials)
    {
      AddVersionRequest inValue = new SIM.Pipelines.SitecoreWebservices.AddVersionRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.AddVersionRequestBody();
      inValue.Body.id = id;
      inValue.Body.language = language;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      AddVersionResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).AddVersion(inValue);
      return retVal.Body.AddVersionResult;
    }

    public XElement CopyTo(string id, string newParent, string name, string databaseName, Credentials credentials)
    {
      CopyToRequest inValue = new SIM.Pipelines.SitecoreWebservices.CopyToRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.CopyToRequestBody();
      inValue.Body.id = id;
      inValue.Body.newParent = newParent;
      inValue.Body.name = name;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      CopyToResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).CopyTo(inValue);
      return retVal.Body.CopyToResult;
    }

    public XElement Delete(string id, bool recycle, string databaseName, Credentials credentials)
    {
      DeleteRequest inValue = new SIM.Pipelines.SitecoreWebservices.DeleteRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.DeleteRequestBody();
      inValue.Body.id = id;
      inValue.Body.recycle = recycle;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      DeleteResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).Delete(inValue);
      return retVal.Body.DeleteResult;
    }

    public XElement DeleteChildren(string id, string databaseName, Credentials credentials)
    {
      DeleteChildrenRequest inValue = new SIM.Pipelines.SitecoreWebservices.DeleteChildrenRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.DeleteChildrenRequestBody();
      inValue.Body.id = id;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      DeleteChildrenResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).DeleteChildren(inValue);
      return retVal.Body.DeleteChildrenResult;
    }

    public XElement Duplicate(string id, string name, string databaseName, Credentials credentials)
    {
      DuplicateRequest inValue = new SIM.Pipelines.SitecoreWebservices.DuplicateRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.DuplicateRequestBody();
      inValue.Body.id = id;
      inValue.Body.name = name;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      DuplicateResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).Duplicate(inValue);
      return retVal.Body.DuplicateResult;
    }

    public XElement GetChildren(string id, string databaseName, Credentials credentials)
    {
      GetChildrenRequest inValue = new SIM.Pipelines.SitecoreWebservices.GetChildrenRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.GetChildrenRequestBody();
      inValue.Body.id = id;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      GetChildrenResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).GetChildren(inValue);
      return retVal.Body.GetChildrenResult;
    }

    public XElement GetDatabases(Credentials credentials)
    {
      GetDatabasesRequest inValue = new SIM.Pipelines.SitecoreWebservices.GetDatabasesRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.GetDatabasesRequestBody();
      inValue.Body.credentials = credentials;
      GetDatabasesResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).GetDatabases(inValue);
      return retVal.Body.GetDatabasesResult;
    }

    public XElement GetItemFields(string id, string language, string version, bool allFields, string databaseName, Credentials credentials)
    {
      GetItemFieldsRequest inValue = new SIM.Pipelines.SitecoreWebservices.GetItemFieldsRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.GetItemFieldsRequestBody();
      inValue.Body.id = id;
      inValue.Body.language = language;
      inValue.Body.version = version;
      inValue.Body.allFields = allFields;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      GetItemFieldsResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).GetItemFields(inValue);
      return retVal.Body.GetItemFieldsResult;
    }

    public XElement GetItemMasters(string id, string databaseName, Credentials credentials)
    {
      GetItemMastersRequest inValue = new SIM.Pipelines.SitecoreWebservices.GetItemMastersRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.GetItemMastersRequestBody();
      inValue.Body.id = id;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      GetItemMastersResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).GetItemMasters(inValue);
      return retVal.Body.GetItemMastersResult;
    }

    public XElement GetLanguages(string databaseName, Credentials credentials)
    {
      GetLanguagesRequest inValue = new SIM.Pipelines.SitecoreWebservices.GetLanguagesRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.GetLanguagesRequestBody();
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      GetLanguagesResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).GetLanguages(inValue);
      return retVal.Body.GetLanguagesResult;
    }

    public XElement GetMasters(string databaseName, Credentials credentials)
    {
      GetMastersRequest inValue = new SIM.Pipelines.SitecoreWebservices.GetMastersRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.GetMastersRequestBody();
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      GetMastersResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).GetMasters(inValue);
      return retVal.Body.GetMastersResult;
    }

    public XElement GetTemplates(string databaseName, Credentials credentials)
    {
      GetTemplatesRequest inValue = new SIM.Pipelines.SitecoreWebservices.GetTemplatesRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.GetTemplatesRequestBody();
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      GetTemplatesResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).GetTemplates(inValue);
      return retVal.Body.GetTemplatesResult;
    }

    public XElement GetXML(string id, bool deep, string databaseName, Credentials credentials)
    {
      GetXMLRequest inValue = new SIM.Pipelines.SitecoreWebservices.GetXMLRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.GetXMLRequestBody();
      inValue.Body.id = id;
      inValue.Body.deep = deep;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      GetXMLResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).GetXML(inValue);
      return retVal.Body.GetXMLResult;
    }

    public XElement InsertXML(string id, string xml, bool changeIDs, string databaseName, Credentials credentials)
    {
      InsertXMLRequest inValue = new SIM.Pipelines.SitecoreWebservices.InsertXMLRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.InsertXMLRequestBody();
      inValue.Body.id = id;
      inValue.Body.xml = xml;
      inValue.Body.changeIDs = changeIDs;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      InsertXMLResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).InsertXML(inValue);
      return retVal.Body.InsertXMLResult;
    }

    public XElement MoveTo(string id, string newParent, string databaseName, Credentials credentials)
    {
      MoveToRequest inValue = new SIM.Pipelines.SitecoreWebservices.MoveToRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.MoveToRequestBody();
      inValue.Body.id = id;
      inValue.Body.newParent = newParent;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      MoveToResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).MoveTo(inValue);
      return retVal.Body.MoveToResult;
    }

    public XElement RemoveVersion(string id, string language, string version, string databaseName, Credentials credentials)
    {
      RemoveVersionRequest inValue = new SIM.Pipelines.SitecoreWebservices.RemoveVersionRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.RemoveVersionRequestBody();
      inValue.Body.id = id;
      inValue.Body.language = language;
      inValue.Body.version = version;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      RemoveVersionResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).RemoveVersion(inValue);
      return retVal.Body.RemoveVersionResult;
    }

    public XElement Rename(string id, string newName, string databaseName, Credentials credentials)
    {
      RenameRequest inValue = new SIM.Pipelines.SitecoreWebservices.RenameRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.RenameRequestBody();
      inValue.Body.id = id;
      inValue.Body.newName = newName;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      RenameResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).Rename(inValue);
      return retVal.Body.RenameResult;
    }

    public XElement Save(string xml, string databaseName, Credentials credentials)
    {
      SaveRequest inValue = new SIM.Pipelines.SitecoreWebservices.SaveRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.SaveRequestBody();
      inValue.Body.xml = xml;
      inValue.Body.databaseName = databaseName;
      inValue.Body.credentials = credentials;
      SaveResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).Save(inValue);
      return retVal.Body.SaveResult;
    }

    public XElement VerifyCredentials(Credentials credentials)
    {
      VerifyCredentialsRequest inValue = new SIM.Pipelines.SitecoreWebservices.VerifyCredentialsRequest();
      inValue.Body = new SIM.Pipelines.SitecoreWebservices.VerifyCredentialsRequestBody();
      inValue.Body.credentials = credentials;
      VerifyCredentialsResponse retVal = ((SIM.Pipelines.SitecoreWebservices.VisualSitecoreServiceSoap)this).VerifyCredentials(inValue);
      return retVal.Body.VerifyCredentialsResult;
    }

    #endregion
  }
}