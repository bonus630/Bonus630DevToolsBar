<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
								xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
								xmlns:frmwrk="Corel Framework Data"
								exclude-result-prefixes="frmwrk">
	<xsl:output method="xml" encoding="UTF-8" indent="yes"/>

	<!-- Use these elements for the framework to move the container from the app config file to the user config file -->
	<!-- Since these elements use the frmwrk name space, they will not be executed when the XSLT is applied to the user config file -->
	<frmwrk:uiconfig>
		<!-- The Application Info should always be the topmost frmwrk element -->
		<!--<frmwrk:compositeNode xPath="/uiConfig/commandBars/commandBarData[@guid='1056b8d8-9185-46ee-af8e-77c7ba383a4e']"/>-->
		<frmwrk:compositeNode xPath="/uiConfig/commandBars/commandBarData[@guid='c2b44f69-6dec-444e-a37e-5dbf7ff43dae']"/>
		<frmwrk:compositeNode xPath="/uiConfig/views/viewTemplate[@guid='ab303a90-464d-5191-423f-613c4d1dcb2c']"/>
		<frmwrk:compositeNode xPath="/uiConfig/frame"/>
	</frmwrk:uiconfig>



	<!-- Copy everything -->
	<xsl:template match="node()|@*">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="commandBarData[@guid='48024933-d18b-4e0a-9b59-96a6b99a418e']/toolbar">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>
			<!--Run Command Docker Button-->
			<xsl:if test="not(./item[@guidRef='7acb54e6-084e-494f-ad31-2718f34ddad2'])">
				<item guidRef="7acb54e6-084e-494f-ad31-2718f34ddad2"/>
			</xsl:if>
			<xsl:if test="not(./item[@guidRef='266435b4-6e53-460f-9fa7-f45be187d400'])">
				<item guidRef="266435b4-6e53-460f-9fa7-f45be187d400"/>
			</xsl:if>
			<!--DrawUI Explorer-->
			<xsl:if test="not(./item[@guidRef='942df043-e3e2-4d0e-83ee-a456557ac093'])">
				<item guidRef="942df043-e3e2-4d0e-83ee-a456557ac093"/>
			</xsl:if>
			
			
			<!--Shortcuts-->
			<xsl:if test="not(./item[@guidRef='b0bc23ba-086b-46d1-ae04-ee36aa9003a4'])">
				<item guidRef="b0bc23ba-086b-46d1-ae04-ee36aa9003a4"/>
			</xsl:if>
			<xsl:if test="not(./item[@guidRef='266435b4-6e53-460f-9fa7-f45be187d400'])">
				<item guidRef="266435b4-6e53-460f-9fa7-f45be187d400"/>
			</xsl:if>
			<!--Gms Dragger-->
			<xsl:if test="not(./item[@guidRef='3b3f72b8-0129-4316-b8a3-40e4758ba9bc'])">
				<item guidRef="3b3f72b8-0129-4316-b8a3-40e4758ba9bc"/>
			</xsl:if>
			<!--Macro Manager-->
				<xsl:if test="not(./item[@guidRef='13d17830-0ba8-4f71-85e0-6df0a1051eee'])">
				<item guidRef="13d17830-0ba8-4f71-85e0-6df0a1051eee"/>
			</xsl:if>
			<!--CommandBar Builder-->
				<xsl:if test="not(./item[@guidRef='f9691f7a-27cc-405b-b2f4-de164246bcbd'])">
				<item guidRef="f9691f7a-27cc-405b-b2f4-de164246bcbd"/>
			</xsl:if>
			<!--Reload GMS-->
			<xsl:if test="not(./item[@guidRef='9b07d7af-da14-4cd8-9db9-7da214ee1d4a'])">
				<item guidRef="9b07d7af-da14-4cd8-9db9-7da214ee1d4a"/>
			</xsl:if>
			
			<!--Unload and Delete GMS-->
			<xsl:if test="not(./item[@guidRef='571db7b2-8cae-4b99-b241-a56ecd61f90e'])">
				<item guidRef="571db7b2-8cae-4b99-b241-a56ecd61f90e"/>
			</xsl:if>

			<xsl:if test="not(./item[@guidRef='266435b4-6e53-460f-9fa7-f45be187d400'])">
				<item guidRef="266435b4-6e53-460f-9fa7-f45be187d400"/>
			</xsl:if>
			<!--CQL Runner-->
			<xsl:if test="not(./item[@guidRef='51b24cfb-b8df-411f-886c-4c5520e931a4'])">
				<item guidRef="51b24cfb-b8df-411f-886c-4c5520e931a4"/>
			</xsl:if>	
			<!--Cql help button-->
			<xsl:if test="not(./item[@guidRef='d61f1ede-79aa-4255-8f96-307e6f63c204'])">
				<item guidRef="d61f1ede-79aa-4255-8f96-307e6f63c204"/>
			</xsl:if>
			<!--Folders-->
			<xsl:if test="not(./item[@guidRef='680d03b3-2da0-4314-bc79-fa6b26471e22'])">
				<item guidRef="680d03b3-2da0-4314-bc79-fa6b26471e22"/>
			</xsl:if>
			<!--IconCreator-->
			<xsl:if test="not(./item[@guidRef='d0a371e7-9fad-4e1c-8159-b285d67c0497'])">
				<item guidRef="d0a371e7-9fad-4e1c-8159-b285d67c0497"/>
			</xsl:if>
			<!--IconLoader-->
			<xsl:if test="not(./item[@guidRef='657042cb-3594-43a1-80bf-c8a27fd43146'])">
				<item guidRef="657042cb-3594-43a1-80bf-c8a27fd43146"/>
			</xsl:if>
			<!--Recent Files-->
			<xsl:if test="not(./item[@guidRef='7d15e9c7-2431-4841-a5aa-9eaa5b581230'])">
				<item dock="fill" guidRef="7d15e9c7-2431-4841-a5aa-9eaa5b581230"/>
			</xsl:if>

		


		</xsl:copy>
	</xsl:template>

	<xsl:template match="uiConfig/containers/container[@guid='bee85f91-3ad9-dc8d-48b5-d2a87c8b2109']/container[@guid='Framework_MainFrame-layout']/dockHost[@guid='894bf987-2ec1-8f83-41d8-68f6797d0db4']/toolbar[@guidRef='c2b44f69-6dec-444e-a37e-5dbf7ff43dae']">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>

			<xsl:if test="not(./toolbar[@guidRef='48024933-d18b-4e0a-9b59-96a6b99a418e'])">
				<toolbar guidRef="48024933-d18b-4e0a-9b59-96a6b99a418e" dock="fill"/>
			</xsl:if>
		</xsl:copy>
	</xsl:template>


	<xsl:template match="uiConfig/states/state[1]/container[@guidRef='bee85f91-3ad9-dc8d-48b5-d2a87c8b2109']/layout/dockHost[@guid='894bf987-2ec1-8f83-41d8-68f6797d0db4']/toolbar[@guidRef='c2b44f69-6dec-444e-a37e-5dbf7ff43dae']">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>

			<xsl:if test="not(./toolbar[@guidRef='48024933-d18b-4e0a-9b59-96a6b99a418e'])">
				<toolbar guidRef="48024933-d18b-4e0a-9b59-96a6b99a418e" dock="fill"/>
			</xsl:if>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="/uiConfig/containers/container[@guid='bee85f91-3ad9-dc8d-48b5-d2a87c8b2109']/container[@guid='Framework_MainFrame-layout']/dockHost/dockHost/dockHost/dockHost[@guid='930211f2-174f-2783-47c8-ac28b179bac7']">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>

			<xsl:if test="not(./viewHost[@guidRef='60b73b65-2952-43b3-95b4-bcfd77a767e1'])">
				<viewHost guid="60b73b65-2952-43b3-95b4-bcfd77a767e1" category="view+docker" selectedView="488c069a-7535-4af9-9c88-eda17c4808f7" dock="bottom">

					<dockerData guidRef="488c069a-7535-4af9-9c88-eda17c4808f7" category="view+docker"/>
				</viewHost>
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="uiConfig/dialogs">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>
			<!-- Pop up CQL Runner-->
			<xsl:if test="not(./dialog[@guidRef='9b6ec438-14c9-44e2-92c3-c28411f093af'])">
				<dialog guidRef="9b6ec438-14c9-44e2-92c3-c28411f093af" dock="top"/>
			</xsl:if>
			<!-- Pop up Folder-->
			<xsl:if test="not(./dialog[@guidRef='b83ecdef-7b82-46ee-bef3-d874902e031a'])">
				<dialog guidRef="b83ecdef-7b82-46ee-bef3-d874902e031a" dock="top"/>
			</xsl:if>
			<!-- Pop up GMS Loader-->
			<xsl:if test="not(./dialog[@guidRef='acdc8c23-25cb-49a7-9346-0a6b69bbf5c9'])">
				<dialog guidRef="acdc8c23-25cb-49a7-9346-0a6b69bbf5c9" dock="top"/>
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<!-- Pop up CQL Runner-->
	<xsl:template match="uiConfig/dialogs/dialog[@guid='9b6ec438-14c9-44e2-92c3-c28411f093af']">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>

			<xsl:if test="not(./item[@guidRef='8a8ca94c-cc61-4f14-b24d-cbd447d2fd56'])">
				<item guidRef="8a8ca94c-cc61-4f14-b24d-cbd447d2fd56" dock="fill"/>
			</xsl:if>

		</xsl:copy>
	</xsl:template>
	<!-- Pop up Folder-->
		<xsl:template match="uiConfig/dialogs/dialog[@guid='b83ecdef-7b82-46ee-bef3-d874902e031a']">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>

			<xsl:if test="not(./item[@guidRef='d13b83a4-3ef6-4ead-b95d-44d467dc47f5'])">
				<item guidRef="d13b83a4-3ef6-4ead-b95d-44d467dc47f5" dock="fill"/>
			</xsl:if>	
		
		</xsl:copy>
	</xsl:template>
	<!-- Pop up  GMS Loader-->
	<xsl:template match="uiConfig/dialogs/dialog[@guid='acdc8c23-25cb-49a7-9346-0a6b69bbf5c9']">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>

			<xsl:if test="not(./item[@guidRef='b0a4b2ff-7bf5-47c3-a92a-16e2a4520746'])">
				<item guidRef="b0a4b2ff-7bf5-47c3-a92a-16e2a4520746" dock="fill"/>
			</xsl:if>

		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>