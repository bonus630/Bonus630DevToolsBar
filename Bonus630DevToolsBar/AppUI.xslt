<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:frmwrk="Corel Framework Data">
	<xsl:output method="xml" encoding="UTF-8" indent="yes"/>

	<!-- Use these elements for the framework to move the container from the app config file to the user config file -->
	<!-- Since these elements use the frmwrk name space, they will not be executed when the XSLT is applied to the user config file -->
	<frmwrk:uiconfig>
		<!-- The Application Info should always be the topmost frmwrk element -->
		<frmwrk:applicationInfo userConfiguration="true" />
	</frmwrk:uiconfig>

	<!-- Copy everything -->
	<xsl:template match="node()|@*">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="uiConfig/items">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>


			<!--Creates a item to add a Warpper, responsible to initialize the DataSource to be consumimed in menu item above -->
			<itemData guid="9d86b3ac-5c08-4c95-9c22-288173525877"
					  type="wpfhost"
					  hostedType="Addons\Bonus630DevToolsBar\Bonus630DevToolsBar.dll,br.com.Bonus630DevToolsBar.ControlUI"
					  enable="false"
					  caption="Bonus630DevToolBar"
					  nonLocalizableName="Bonus630DevToolBar"
			/>

			<!-- Run Command Docker Button onInvoke="*Bind(DataSource=Bonus630DevToolsBarDS;Path=RunCommandDocker)"-->
			<itemData guid="7acb54e6-084e-494f-ad31-2718f34ddad2"
					 check="*Docker('5087687d-337d-4d0e-acaf-c0b1df967757')"
					  icon="guid://7acb54e6-084e-494f-ad31-2718f34ddad2"
					  type="checkButton"  enable="true"/>

			<!-- IconCreatorHelper button -->
			<!--<itemData guid="d0a371e7-9fad-4e1c-8159-b285d67c0497"
					 onInvoke="*Bind(DataSource=Bonus630DevToolsBarDS;Path=RunIconCreatorHelper)"
					  icon="guid://d0a371e7-9fad-4e1c-8159-b285d67c0497"
					  type="button"  enable="true" />-->
			<!-- IconCreatorHelper button -->
			<itemData guid="d0a371e7-9fad-4e1c-8159-b285d67c0497"
					 check="*Docker('488c069a-7535-4af9-9c88-eda17c4808f7')"
					  icon="guid://d0a371e7-9fad-4e1c-8159-b285d67c0497"
					  type="checkButton"  enable="true"/>
			
			<!-- IconCreatorHelper Loader button-->
			<itemData guid="657042cb-3594-43a1-80bf-c8a27fd43146"
					  onInvoke="*Bind(DataSource=Bonus630DevToolsBarDS;Path=LoadIcon)"
					  icon="guid://657042cb-3594-43a1-80bf-c8a27fd43146"
					  caption="Select a icon"		  
					  type="button"  enable="true"/>
			<!---->

			<!-- DrawUI Explorer Button-->
			<itemData guid="942df043-e3e2-4d0e-83ee-a456557ac093"
					  onInvoke="*Bind(DataSource=Bonus630DevToolsBarDS;Path=RunDrawUIExplorer)"
					  icon="guid://942df043-e3e2-4d0e-83ee-a456557ac093"
					  type="button"  enable="true"/>

			<!-- Shortcuts Button onInvoke="*Bind(DataSource=Bonus630DevToolsBarDS;Path=RunShortcutsDocker)"-->
			<itemData guid="b0bc23ba-086b-46d1-ae04-ee36aa9003a4"
					  check="*Docker('512194ae-a540-4979-8991-bcadded6726e')"
					  icon="guid://b0bc23ba-086b-46d1-ae04-ee36aa9003a4"
					  type="checkButton"  enable="true"/>

			<!--Run command docker-->
			<itemData guid="2ee3372b-f6b5-47fe-aa81-4ecd2c4771e2"
					type="wpfhost"
					hostedType="Addons\Bonus630DevToolsBar\Bonus630DevToolsBar.dll,br.com.Bonus630DevToolsBar.RunCommandDocker.DockerUI"
					enable="true"/>

			<!--Shortcuts docker-->
			<itemData guid="c934c5dc-882b-476a-a634-b1d9ab7c81f1"
					type="wpfhost"
					hostedType="Addons\Bonus630DevToolsBar\Bonus630DevToolsBar.dll,br.com.Bonus630DevToolsBar.ControlsShorcutsCDRAddon.DockerUI"
					enable="true"/>

			<!--GMS Dragger-->
			<itemData guid="3b3f72b8-0129-4316-b8a3-40e4758ba9bc"
					type="wpfhost"
					hostedType="Addons\Bonus630DevToolsBar\Bonus630DevToolsBar.dll,br.com.Bonus630DevToolsBar.GMSDragger.Dragger"
					enable="true"
					width ="*Bind(DataSource=Bonus630DevToolsBarDS;Path=ItemWidth)" 
					height="*Bind(DataSource=Bonus630DevToolsBarDS;Path=ItemWidth)"
					caption="Drag your GMS here!"
			/>

			<!--Reload GMS-->
			<itemData guid="9b07d7af-da14-4cd8-9db9-7da214ee1d4a"
					  onInvoke="*Bind(DataSource=Bonus630DevToolsBarDS;Path=ReloadNRestoreUserGMS)"
					  icon="guid://9b07d7af-da14-4cd8-9db9-7da214ee1d4a"
					  dropDownRef='b0a4b2ff-7bf5-47c3-a92a-16e2a4520746'
					  type="dropDownDlgBtn"  enable="true" arrowStyle='down'/>
			
			<!--PopUp GMS-->
			<itemData guid="b0a4b2ff-7bf5-47c3-a92a-16e2a4520746"
					type="wpfhost"
					hostedType="Addons\Bonus630DevToolsBar\Bonus630DevToolsBar.dll,br.com.Bonus630DevToolsBar.GMSDragger.GMSLoader"
					enable="true"/>
			
			<!-- Unload and Delete GMS-->
			<itemData guid="571db7b2-8cae-4b99-b241-a56ecd61f90e"
					  onInvoke="*Bind(DataSource=Bonus630DevToolsBarDS;Path=UnloadNDeleteUserGMS)"
					  icon="guid://571db7b2-8cae-4b99-b241-a56ecd61f90e"
					  type="button"  enable="true"/>
			
			<!--CQL Runner-->
			<!--<itemData guid="08bdac37-d2b9-42ad-a498-5335df9783c7"
					type="wpfhost"
					hostedType="Addons\Bonus630DevToolsBar\Bonus630DevToolsBar.dll,br.com.Bonus630DevToolsBar.CQLRunner.CQLRunner"
					enable="true"/>-->	
			
			<!--DropdownButton CqL-->
			<itemData guid='51b24cfb-b8df-411f-886c-4c5520e931a4' type='dropDownDlgBtn' arrowStyle='down'
					  caption='*Bind(DataSource=Bonus630DevToolsBarDS;Path=CQLCaption)'
					  toolTip='*Bind(DataSource=Bonus630DevToolsBarDS;Path=CQLTooltip)'
					  dropDownRef='8a8ca94c-cc61-4f14-b24d-cbd447d2fd56'
					  icon="guid://51b24cfb-b8df-411f-886c-4c5520e931a4"
					  length='100' enable='true'/>

			<!--PopUp CQL-->
			<itemData guid="8a8ca94c-cc61-4f14-b24d-cbd447d2fd56"
					type="wpfhost"
					hostedType="Addons\Bonus630DevToolsBar\Bonus630DevToolsBar.dll,br.com.Bonus630DevToolsBar.CQLRunner.CQLRunner"
					enable="true"/>
			
					
			<!--cql reference guid-->
			<itemData guid="d61f1ede-79aa-4255-8f96-307e6f63c204"
			  onInvoke="*Bind(DataSource=Bonus630DevToolsBarDS;Path=OpenCQLGuide)"
			  icon="guid://d61f1ede-79aa-4255-8f96-307e6f63c204"
			  type="button"  enable="true"/>

			<!--DropdownButton Folders-->
			<itemData guid='680d03b3-2da0-4314-bc79-fa6b26471e22' type='dropDownDlgBtn' arrowStyle='down'
					  caption='*Bind(DataSource=Bonus630DevToolsBarDS;Path=FoldersCaption)'
					  toolTip='*Bind(DataSource=Bonus630DevToolsBarDS;Path=FoldersTooltip)'
					  dropDownRef='d13b83a4-3ef6-4ead-b95d-44d467dc47f5'
					  icon="guid://680d03b3-2da0-4314-bc79-fa6b26471e22"
					  length='100' enable='true'/>

			<!--PopUp Folders-->
			<itemData guid="d13b83a4-3ef6-4ead-b95d-44d467dc47f5"
					type="wpfhost"
					hostedType="Addons\Bonus630DevToolsBar\Bonus630DevToolsBar.dll,br.com.Bonus630DevToolsBar.Folders.Folders"
					enable="true"/>

			<!--IconCreatorHelper-->
			<itemData guid="ed9bead6-3acf-43a9-887e-9c5c3f30a681"
					type="wpfhost"
					hostedType="Addons\Bonus630DevToolsBar\Bonus630DevToolsBar.dll,br.com.Bonus630DevToolsBar.IconCreatorHelper.IconCreatorHelperUI"
					enable="true" Width="310"/>
			
			
			<!--Recent Files-->
			<itemData guid="7d15e9c7-2431-4841-a5aa-9eaa5b581230"
					type="wpfhost"
					hostedType="Addons\Bonus630DevToolsBar\Bonus630DevToolsBar.dll,br.com.Bonus630DevToolsBar.RecentFiles.RecentFilesView"
					enable="true"
				
					  />	<!--Height ="*Bind(DataSource=Bonus630DevToolsBarDS;Path=ItemWidth;BindType=TwoWay)"-->




		</xsl:copy>
	</xsl:template>

	<!--Create a custom command bar below standard bar-->
	<xsl:template match="uiConfig/commandBars">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>

			<commandBarData guid="48024933-d18b-4e0a-9b59-96a6b99a418e"
							nonLocalizableName="Bonus630 Dev Tools"
							userCaption="Bonus630 Dev Tools"
							locked="true"
							type="toolbar"
							dock="fill">
				
					<toolbar dock="fill">
						<!--1 Data Source-->
						<item  guidRef="9d86b3ac-5c08-4c95-9c22-288173525877" dock="top"/>
						<!--2 Run Command Docker Button-->
						<item  guidRef="7acb54e6-084e-494f-ad31-2718f34ddad2" dock="top"/>
						<!--3 Separator-->
						<item guidRef="266435b4-6e53-460f-9fa7-f45be187d400" />
						<!--4 DrawUI Explorer Button-->
						<item  guidRef="942df043-e3e2-4d0e-83ee-a456557ac093" />
						<!--5 Shortcut Button-->
						<item  guidRef="b0bc23ba-086b-46d1-ae04-ee36aa9003a4" dock="top"/>
						<!--6 Separator-->
						<item guidRef="266435b4-6e53-460f-9fa7-f45be187d400" />
						<!--7 GMS Dragger-->
						<item  guidRef="3b3f72b8-0129-4316-b8a3-40e4758ba9bc" />
						<!--8 Macro Manager-->
						<item guidRef="13d17830-0ba8-4f71-85e0-6df0a1051eee" dock="top" />
						<!--9 Reload Gms -->
						<item  guidRef="9b07d7af-da14-4cd8-9db9-7da214ee1d4a" dock="top"/>
						<!--10 Unload and Delete GMS -->
						<item  guidRef="571db7b2-8cae-4b99-b241-a56ecd61f90e" dock="top"/>
						<!--11 Separator-->
						<item guidRef="266435b4-6e53-460f-9fa7-f45be187d400" />
					
						<!--12 CqlRunner -->
						<item  guidRef="51b24cfb-b8df-411f-886c-4c5520e931a4" />	
						<!--13 CqlHelp -->
						<item  guidRef="d61f1ede-79aa-4255-8f96-307e6f63c204" />	
						<!--14 Separator-->
						<item guidRef="266435b4-6e53-460f-9fa7-f45be187d400" />
						<!--15 Folders-->
						<item guidRef="680d03b3-2da0-4314-bc79-fa6b26471e22" />
						<!--16 Separator-->
						<item guidRef="266435b4-6e53-460f-9fa7-f45be187d400" />
						
						<!--17 IconCreatorHelper-->
						<item guidRef="d0a371e7-9fad-4e1c-8159-b285d67c0497" />
						<!--18 IconCreatorHelper loader-->
						<item guidRef="657042cb-3594-43a1-80bf-c8a27fd43146" />
						
						
						
						<!--19 Separator-->
						<item guidRef="266435b4-6e53-460f-9fa7-f45be187d400" />
						<!--20 Recent Files -->
						<item  guidRef="7d15e9c7-2431-4841-a5aa-9eaa5b581230" dock="fill"/>
						<!--<item  guidRef="118bad9e-cab3-4810-883e-843626f798ae" dock="top"/>-->

					</toolbar>
				
			</commandBarData>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="uiConfig/containers/container[@guid='bee85f91-3ad9-dc8d-48b5-d2a87c8b2109']/container[@guid='Framework_MainFrame-layout']/dockHost[@guid='894bf987-2ec1-8f83-41d8-68f6797d0db4']/toolbar[@guidRef='c2b44f69-6dec-444e-a37e-5dbf7ff43dae']">
		<xsl:copy-of select="."/>

		<toolbar dock="fill" guidRef="48024933-d18b-4e0a-9b59-96a6b99a418e" />

	</xsl:template>
	<xsl:template match="uiConfig/dockers">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>

			<!-- Define RunCommandDocker -->
			<dockerData guid="5087687d-337d-4d0e-acaf-c0b1df967757"
						userCaption="Run Command"
						wantReturn="true"
						icon="guid://7acb54e6-084e-494f-ad31-2718f34ddad2"
						focusStyle="noThrow">
				<container>
					<!-- add the webpage control to the docker -->
					<item dock="fill" margin="0,0,0,0" guidRef="2ee3372b-f6b5-47fe-aa81-4ecd2c4771e2"/>
				</container>
			</dockerData>

			<!-- Define ShortcutsDocker -->
			<dockerData guid="512194ae-a540-4979-8991-bcadded6726e"
						userCaption="Shortcuts"
						wantReturn="true"
						icon="guid://b0bc23ba-086b-46d1-ae04-ee36aa9003a4"
						focusStyle="noThrow">
				<container>
					<!-- add the webpage control to the docker -->
					<item dock="fill" margin="0,0,0,0" guidRef="c934c5dc-882b-476a-a634-b1d9ab7c81f1"/>
				</container>
			</dockerData>

			<!-- Define IconContainerHelperDocker -->
			<dockerData guid="488c069a-7535-4af9-9c88-eda17c4808f7"
						userCaption="IconContainerHelper"
						wantReturn="true"
						icon="guid://d0a371e7-9fad-4e1c-8159-b285d67c0497"
						focusStyle="noThrow"
						dock="bottom">
				<container>
					<!-- add the webpage control to the docker -->
					<item dock="fill" width="310" margin="0,0,0,0" guidRef="ed9bead6-3acf-43a9-887e-9c5c3f30a681"/>
				</container>
			</dockerData>

		</xsl:copy>
	</xsl:template>
	
	<xsl:template match="uiConfig/dialogs">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>
			<!-- Pop up CQL Runner-->
			<dialog guid="9b6ec438-14c9-44e2-92c3-c28411f093af"
						   popUp="true">

				<item  guidRef="8a8ca94c-cc61-4f14-b24d-cbd447d2fd56" dock="fill"/>

			</dialog>
			<!-- Pop up Folder-->
			<dialog guid="b83ecdef-7b82-46ee-bef3-d874902e031a"
						   popUp="true">

				<item  guidRef="d13b83a4-3ef6-4ead-b95d-44d467dc47f5" dock="fill"/>

			</dialog>
			<!-- Pop up GMS Load-->
			<dialog guid="acdc8c23-25cb-49a7-9346-0a6b69bbf5c9"
						   popUp="true">

				<item  guidRef="b0a4b2ff-7bf5-47c3-a92a-16e2a4520746" dock="fill"/>

			</dialog>

		</xsl:copy>
	</xsl:template>

	<!--<xsl:template match="/uiConfig/containers/container[@guid='bee85f91-3ad9-dc8d-48b5-d2a87c8b2109']/container[@guid='Framework_MainFrame-layout']/dockHost/dockHost/dockHost/dockHost/viewHost[@]">-->
	<!--<xsl:template match="/uiConfig/containers/container[@guid='bee85f91-3ad9-dc8d-48b5-d2a87c8b2109']/container[@guid='Framework_MainFrame-layout']/dockHost/dockHost/dockHost/dockHost/viewHost[@guid='344da698-5b33-46ff-bfdf-4d8bc2906450']">-->
	<xsl:template match="/dockHost[@guid='930211f2-174f-2783-47c8-ac28b179bac7']">
		<xsl:copy-of select="."/>
		<viewHost guid="60b73b65-2952-43b3-95b4-bcfd77a767e1" category="view+docker" selectedView="488c069a-7535-4af9-9c88-eda17c4808f7" dock="bottom">
			
			<dockerData guidRef="488c069a-7535-4af9-9c88-eda17c4808f7" category="view+docker"/>
		</viewHost>

	
	</xsl:template>
	
		<xsl:template match="/uiConfig/customizationList/container">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>
			<modeData guid="9d86b3ac-5c08-4c95-9c22-288173525877" >
				<!--WPF control cant add here, will breaks coreldraw-->
						<!--1 Data Source-->
						<!--<item  guidRef="9d86b3ac-5c08-4c95-9c22-288173525877"/>-->
						<!--2 Run Command Docker Button-->
						<item  guidRef="7acb54e6-084e-494f-ad31-2718f34ddad2" />
					
						<!--4 DrawUI Explorer Button-->
						<item  guidRef="942df043-e3e2-4d0e-83ee-a456557ac093" />
						<!--5 Shortcut Button-->
						<item  guidRef="b0bc23ba-086b-46d1-ae04-ee36aa9003a4" />
					
						<!--7 GMS Dragger-->
						<!--<item  guidRef="3b3f72b8-0129-4316-b8a3-40e4758ba9bc" />-->
						<!--8 Macro Manager-->
						<!--<item guidRef="13d17830-0ba8-4f71-85e0-6df0a1051eee" />-->
						<!--9 Reload Gms -->
						<item  guidRef="9b07d7af-da14-4cd8-9db9-7da214ee1d4a" />
						<!--10 Unload and Delete GMS -->
						<item  guidRef="571db7b2-8cae-4b99-b241-a56ecd61f90e" />
				
					
						<!--12 CqlRunner -->
						<item  guidRef="51b24cfb-b8df-411f-886c-4c5520e931a4" />	
						<!--13 CqlHelp -->
						<item  guidRef="d61f1ede-79aa-4255-8f96-307e6f63c204" />	
						
						<!--15 Folders-->
						<item guidRef="680d03b3-2da0-4314-bc79-fa6b26471e22" />
			
						
						<!--17 IconCreatorHelper-->
						<item guidRef="d0a371e7-9fad-4e1c-8159-b285d67c0497" />
						<!--18 IconCreatorHelper loader-->
						<item guidRef="657042cb-3594-43a1-80bf-c8a27fd43146" />
						
						
						
					
						<!--20 Recent Files -->
						<!--<item  guidRef="7d15e9c7-2431-4841-a5aa-9eaa5b581230" />-->
						
			
			</modeData>

		</xsl:copy>
	</xsl:template>

</xsl:stylesheet>
