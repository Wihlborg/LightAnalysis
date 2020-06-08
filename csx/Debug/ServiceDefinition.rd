<?xml version="1.0" encoding="utf-8"?>
<<<<<<< HEAD
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="LightAnalysis" generation="1" functional="0" release="0" Id="b6d9c81e-9067-48bc-a278-3758bb4e0d50" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
=======
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="LightAnalysis" generation="1" functional="0" release="0" Id="dadec34f-aa6d-40c9-a014-3cf535574cdc" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
>>>>>>> 3d7a0226bf1494f341873206b875ca42a23143bb
  <groups>
    <group name="LightAnalysisGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="Frontend:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/LightAnalysis/LightAnalysisGroup/LB:Frontend:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Backend:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/LightAnalysis/LightAnalysisGroup/MapBackend:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="BackendInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/LightAnalysis/LightAnalysisGroup/MapBackendInstances" />
          </maps>
        </aCS>
        <aCS name="Frontend:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/LightAnalysis/LightAnalysisGroup/MapFrontend:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="FrontendInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/LightAnalysis/LightAnalysisGroup/MapFrontendInstances" />
          </maps>
        </aCS>
        <aCS name="UserAuthentication:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/LightAnalysis/LightAnalysisGroup/MapUserAuthentication:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="UserAuthenticationInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/LightAnalysis/LightAnalysisGroup/MapUserAuthenticationInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:Frontend:Endpoint1">
          <toPorts>
            <inPortMoniker name="/LightAnalysis/LightAnalysisGroup/Frontend/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapBackend:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/LightAnalysis/LightAnalysisGroup/Backend/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapBackendInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/LightAnalysis/LightAnalysisGroup/BackendInstances" />
          </setting>
        </map>
        <map name="MapFrontend:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/LightAnalysis/LightAnalysisGroup/Frontend/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapFrontendInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/LightAnalysis/LightAnalysisGroup/FrontendInstances" />
          </setting>
        </map>
        <map name="MapUserAuthentication:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/LightAnalysis/LightAnalysisGroup/UserAuthentication/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapUserAuthenticationInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/LightAnalysis/LightAnalysisGroup/UserAuthenticationInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
<<<<<<< HEAD
          <role name="Backend" generation="1" functional="0" release="0" software="C:\Users\Oskar\LightAnalysis\csx\Debug\roles\Backend" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
=======
          <role name="Backend" generation="1" functional="0" release="0" software="C:\Users\Ralle\Desktop\New folder (2)\LightAnalysis\csx\Debug\roles\Backend" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
>>>>>>> 3d7a0226bf1494f341873206b875ca42a23143bb
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;Backend&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Backend&quot; /&gt;&lt;r name=&quot;Frontend&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;UserAuthentication&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/LightAnalysis/LightAnalysisGroup/BackendInstances" />
            <sCSPolicyUpdateDomainMoniker name="/LightAnalysis/LightAnalysisGroup/BackendUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/LightAnalysis/LightAnalysisGroup/BackendFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
<<<<<<< HEAD
          <role name="Frontend" generation="1" functional="0" release="0" software="C:\Users\Oskar\LightAnalysis\csx\Debug\roles\Frontend" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
=======
          <role name="Frontend" generation="1" functional="0" release="0" software="C:\Users\Ralle\Desktop\New folder (2)\LightAnalysis\csx\Debug\roles\Frontend" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
>>>>>>> 3d7a0226bf1494f341873206b875ca42a23143bb
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;Frontend&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Backend&quot; /&gt;&lt;r name=&quot;Frontend&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;UserAuthentication&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/LightAnalysis/LightAnalysisGroup/FrontendInstances" />
            <sCSPolicyUpdateDomainMoniker name="/LightAnalysis/LightAnalysisGroup/FrontendUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/LightAnalysis/LightAnalysisGroup/FrontendFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
<<<<<<< HEAD
          <role name="UserAuthentication" generation="1" functional="0" release="0" software="C:\Users\Oskar\LightAnalysis\csx\Debug\roles\UserAuthentication" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
=======
          <role name="UserAuthentication" generation="1" functional="0" release="0" software="C:\Users\Ralle\Desktop\New folder (2)\LightAnalysis\csx\Debug\roles\UserAuthentication" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
>>>>>>> 3d7a0226bf1494f341873206b875ca42a23143bb
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;UserAuthentication&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Backend&quot; /&gt;&lt;r name=&quot;Frontend&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;UserAuthentication&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/LightAnalysis/LightAnalysisGroup/UserAuthenticationInstances" />
            <sCSPolicyUpdateDomainMoniker name="/LightAnalysis/LightAnalysisGroup/UserAuthenticationUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/LightAnalysis/LightAnalysisGroup/UserAuthenticationFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="FrontendUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="BackendUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="UserAuthenticationUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="BackendFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="FrontendFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="UserAuthenticationFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="BackendInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="FrontendInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="UserAuthenticationInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
<<<<<<< HEAD
    <implementation Id="26d14afa-3df8-424c-809a-52708c8da7e2" ref="Microsoft.RedDog.Contract\ServiceContract\LightAnalysisContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="b98e2587-9536-46c7-874d-fba4530a3494" ref="Microsoft.RedDog.Contract\Interface\Frontend:Endpoint1@ServiceDefinition">
=======
    <implementation Id="5e2ad9a2-9a10-44bc-900f-f11a0a0d7f59" ref="Microsoft.RedDog.Contract\ServiceContract\LightAnalysisContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="5978d205-7e07-4b0e-8f11-93affd09eade" ref="Microsoft.RedDog.Contract\Interface\Frontend:Endpoint1@ServiceDefinition">
>>>>>>> 3d7a0226bf1494f341873206b875ca42a23143bb
          <inPort>
            <inPortMoniker name="/LightAnalysis/LightAnalysisGroup/Frontend:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>