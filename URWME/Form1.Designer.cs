
namespace URWME
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            cmsMenu = new System.Windows.Forms.ContextMenuStrip(components);
            tsmiCraftingMenu = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem20 = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCheatMenu = new System.Windows.Forms.ToolStripMenuItem();
            tsmiEditorMenu = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCharacterEditor = new System.Windows.Forms.ToolStripMenuItem();
            tsmiInventoryEditor = new System.Windows.Forms.ToolStripMenuItem();
            tsmiItemEditor = new System.Windows.Forms.ToolStripMenuItem();
            tsmiInjuryEditor = new System.Windows.Forms.ToolStripMenuItem();
            tsmiPassiveCheats = new System.Windows.Forms.ToolStripMenuItem();
            tsmiNoInjuries = new System.Windows.Forms.ToolStripMenuItem();
            tsmiNoCarryWeight = new System.Windows.Forms.ToolStripMenuItem();
            tsmiNoNeeds = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTreeVision = new System.Windows.Forms.ToolStripMenuItem();
            tsmiCannibalism = new System.Windows.Forms.ToolStripMenuItem();
            tsmiFreezeNPC = new System.Windows.Forms.ToolStripMenuItem();
            tsmiFreezeTime = new System.Windows.Forms.ToolStripMenuItem();
            tsmiSpawnMenu = new System.Windows.Forms.ToolStripMenuItem();
            tsmiSpawnItem = new System.Windows.Forms.ToolStripMenuItem();
            tscbSpawnItemSelect = new System.Windows.Forms.ToolStripComboBox();
            tstbSpawnItemQuantity = new System.Windows.Forms.ToolStripTextBox();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            tsmiSpawnItemName = new System.Windows.Forms.ToolStripMenuItem();
            tsmiSpawnItemType = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            tsmiSpawnItemButton = new System.Windows.Forms.ToolStripMenuItem();
            tsmiSpawnNPC = new System.Windows.Forms.ToolStripMenuItem();
            toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            tsmiSpawnObject = new System.Windows.Forms.ToolStripMenuItem();
            toolStripComboBox2 = new System.Windows.Forms.ToolStripComboBox();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            tsmiEditHereMenu = new System.Windows.Forms.ToolStripMenuItem();
            tsmiEditItem = new System.Windows.Forms.ToolStripMenuItem();
            tsmiEditNpc = new System.Windows.Forms.ToolStripMenuItem();
            tsmiEditObject = new System.Windows.Forms.ToolStripMenuItem();
            tsmiEditTile = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTeleportMenu = new System.Windows.Forms.ToolStripMenuItem();
            tsmiWaypointMenu = new System.Windows.Forms.ToolStripMenuItem();
            tsmiAddWaypoint = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTeleportToClosest = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTeleportToHere = new System.Windows.Forms.ToolStripMenuItem();
            tsmiMiscMenu = new System.Windows.Forms.ToolStripMenuItem();
            tsmiChecksum = new System.Windows.Forms.ToolStripMenuItem();
            tsmiGenerateCT = new System.Windows.Forms.ToolStripMenuItem();
            tsmiTeleport = new System.Windows.Forms.ToolStripMenuItem();
            tsmiDirection = new System.Windows.Forms.ToolStripMenuItem();
            tsmiGetItems = new System.Windows.Forms.ToolStripMenuItem();
            tsmiSettings = new System.Windows.Forms.ToolStripMenuItem();
            tsmiFont = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            tsmiCloseMenu = new System.Windows.Forms.ToolStripMenuItem();
            panel1 = new System.Windows.Forms.Panel();
            cmsMenu.SuspendLayout();
            SuspendLayout();
            // 
            // cmsMenu
            // 
            cmsMenu.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            cmsMenu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            cmsMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            cmsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiCraftingMenu, tsmiCheatMenu, tsmiTeleport, tsmiDirection, tsmiGetItems, tsmiSettings, toolStripSeparator7, tsmiCloseMenu });
            cmsMenu.Name = "contextMenuStrip1";
            cmsMenu.Size = new System.Drawing.Size(211, 206);
            cmsMenu.Opening += cmsMenu_Opening;
            cmsMenu.PreviewKeyDown += cmsMenu_PreviewKeyDown;
            // 
            // tsmiCraftingMenu
            // 
            tsmiCraftingMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMenuItem20 });
            tsmiCraftingMenu.Enabled = false;
            tsmiCraftingMenu.Name = "tsmiCraftingMenu";
            tsmiCraftingMenu.Size = new System.Drawing.Size(210, 24);
            tsmiCraftingMenu.Text = "Crafting";
            tsmiCraftingMenu.Visible = false;
            // 
            // toolStripMenuItem20
            // 
            toolStripMenuItem20.Name = "toolStripMenuItem20";
            toolStripMenuItem20.Size = new System.Drawing.Size(233, 26);
            toolStripMenuItem20.Text = "toolStripMenuItem20";
            // 
            // tsmiCheatMenu
            // 
            tsmiCheatMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiEditorMenu, tsmiPassiveCheats, tsmiSpawnMenu, tsmiEditHereMenu, tsmiTeleportMenu, tsmiMiscMenu });
            tsmiCheatMenu.Name = "tsmiCheatMenu";
            tsmiCheatMenu.Size = new System.Drawing.Size(210, 24);
            tsmiCheatMenu.Text = "Cheats";
            // 
            // tsmiEditorMenu
            // 
            tsmiEditorMenu.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiEditorMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiCharacterEditor, tsmiInventoryEditor, tsmiItemEditor, tsmiInjuryEditor });
            tsmiEditorMenu.Name = "tsmiEditorMenu";
            tsmiEditorMenu.Size = new System.Drawing.Size(224, 26);
            tsmiEditorMenu.Text = "Editors";
            // 
            // tsmiCharacterEditor
            // 
            tsmiCharacterEditor.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiCharacterEditor.Name = "tsmiCharacterEditor";
            tsmiCharacterEditor.Size = new System.Drawing.Size(155, 26);
            tsmiCharacterEditor.Text = "Character";
            tsmiCharacterEditor.Click += tsmiItem_Click;
            // 
            // tsmiInventoryEditor
            // 
            tsmiInventoryEditor.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiInventoryEditor.Enabled = false;
            tsmiInventoryEditor.Name = "tsmiInventoryEditor";
            tsmiInventoryEditor.Size = new System.Drawing.Size(155, 26);
            tsmiInventoryEditor.Text = "Inventory";
            tsmiInventoryEditor.Click += tsmiItem_Click;
            // 
            // tsmiItemEditor
            // 
            tsmiItemEditor.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiItemEditor.Name = "tsmiItemEditor";
            tsmiItemEditor.Size = new System.Drawing.Size(155, 26);
            tsmiItemEditor.Text = "Item";
            tsmiItemEditor.Click += tsmiItem_Click;
            // 
            // tsmiInjuryEditor
            // 
            tsmiInjuryEditor.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiInjuryEditor.Enabled = false;
            tsmiInjuryEditor.Name = "tsmiInjuryEditor";
            tsmiInjuryEditor.Size = new System.Drawing.Size(155, 26);
            tsmiInjuryEditor.Text = "Injuries";
            tsmiInjuryEditor.Click += tsmiItem_Click;
            // 
            // tsmiPassiveCheats
            // 
            tsmiPassiveCheats.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiPassiveCheats.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiNoInjuries, tsmiNoCarryWeight, tsmiNoNeeds, tsmiTreeVision, tsmiCannibalism, tsmiFreezeNPC, tsmiFreezeTime });
            tsmiPassiveCheats.Name = "tsmiPassiveCheats";
            tsmiPassiveCheats.Size = new System.Drawing.Size(224, 26);
            tsmiPassiveCheats.Text = "Passive";
            // 
            // tsmiNoInjuries
            // 
            tsmiNoInjuries.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiNoInjuries.CheckOnClick = true;
            tsmiNoInjuries.Enabled = false;
            tsmiNoInjuries.Name = "tsmiNoInjuries";
            tsmiNoInjuries.Size = new System.Drawing.Size(205, 26);
            tsmiNoInjuries.Text = "No Injuries";
            tsmiNoInjuries.CheckedChanged += tsmiItem_CheckedChanged;
            // 
            // tsmiNoCarryWeight
            // 
            tsmiNoCarryWeight.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiNoCarryWeight.CheckOnClick = true;
            tsmiNoCarryWeight.Name = "tsmiNoCarryWeight";
            tsmiNoCarryWeight.Size = new System.Drawing.Size(205, 26);
            tsmiNoCarryWeight.Text = "No Carry Weight";
            tsmiNoCarryWeight.CheckedChanged += tsmiItem_CheckedChanged;
            // 
            // tsmiNoNeeds
            // 
            tsmiNoNeeds.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiNoNeeds.CheckOnClick = true;
            tsmiNoNeeds.Name = "tsmiNoNeeds";
            tsmiNoNeeds.Size = new System.Drawing.Size(205, 26);
            tsmiNoNeeds.Text = "No Needs";
            tsmiNoNeeds.CheckedChanged += tsmiItem_CheckedChanged;
            // 
            // tsmiTreeVision
            // 
            tsmiTreeVision.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiTreeVision.CheckOnClick = true;
            tsmiTreeVision.Name = "tsmiTreeVision";
            tsmiTreeVision.Size = new System.Drawing.Size(205, 26);
            tsmiTreeVision.Text = "Xray / Tree vision";
            tsmiTreeVision.CheckedChanged += tsmiItem_CheckedChanged;
            // 
            // tsmiCannibalism
            // 
            tsmiCannibalism.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiCannibalism.CheckOnClick = true;
            tsmiCannibalism.Name = "tsmiCannibalism";
            tsmiCannibalism.Size = new System.Drawing.Size(205, 26);
            tsmiCannibalism.Text = "Cannibalism";
            tsmiCannibalism.CheckedChanged += tsmiItem_CheckedChanged;
            // 
            // tsmiFreezeNPC
            // 
            tsmiFreezeNPC.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiFreezeNPC.CheckOnClick = true;
            tsmiFreezeNPC.Enabled = false;
            tsmiFreezeNPC.Name = "tsmiFreezeNPC";
            tsmiFreezeNPC.Size = new System.Drawing.Size(205, 26);
            tsmiFreezeNPC.Text = "Freeze NPCs";
            tsmiFreezeNPC.CheckedChanged += tsmiItem_CheckedChanged;
            // 
            // tsmiFreezeTime
            // 
            tsmiFreezeTime.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiFreezeTime.CheckOnClick = true;
            tsmiFreezeTime.Enabled = false;
            tsmiFreezeTime.Name = "tsmiFreezeTime";
            tsmiFreezeTime.Size = new System.Drawing.Size(205, 26);
            tsmiFreezeTime.Text = "Freeze Time";
            tsmiFreezeTime.CheckedChanged += tsmiItem_CheckedChanged;
            // 
            // tsmiSpawnMenu
            // 
            tsmiSpawnMenu.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiSpawnMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiSpawnItem, tsmiSpawnNPC, tsmiSpawnObject });
            tsmiSpawnMenu.Enabled = false;
            tsmiSpawnMenu.Name = "tsmiSpawnMenu";
            tsmiSpawnMenu.Size = new System.Drawing.Size(224, 26);
            tsmiSpawnMenu.Text = "Spawn";
            // 
            // tsmiSpawnItem
            // 
            tsmiSpawnItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tscbSpawnItemSelect, tstbSpawnItemQuantity, toolStripSeparator1, tsmiSpawnItemName, tsmiSpawnItemType, toolStripSeparator4, tsmiSpawnItemButton });
            tsmiSpawnItem.Name = "tsmiSpawnItem";
            tsmiSpawnItem.Size = new System.Drawing.Size(136, 26);
            tsmiSpawnItem.Text = "Item";
            // 
            // tscbSpawnItemSelect
            // 
            tscbSpawnItemSelect.Name = "tscbSpawnItemSelect";
            tscbSpawnItemSelect.Size = new System.Drawing.Size(121, 28);
            // 
            // tstbSpawnItemQuantity
            // 
            tstbSpawnItemQuantity.Name = "tstbSpawnItemQuantity";
            tstbSpawnItemQuantity.Size = new System.Drawing.Size(100, 27);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(222, 6);
            // 
            // tsmiSpawnItemName
            // 
            tsmiSpawnItemName.Name = "tsmiSpawnItemName";
            tsmiSpawnItemName.Size = new System.Drawing.Size(225, 26);
            tsmiSpawnItemName.Text = "toolStripMenuItem1";
            // 
            // tsmiSpawnItemType
            // 
            tsmiSpawnItemType.Name = "tsmiSpawnItemType";
            tsmiSpawnItemType.Size = new System.Drawing.Size(225, 26);
            tsmiSpawnItemType.Text = "toolStripMenuItem3";
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(222, 6);
            // 
            // tsmiSpawnItemButton
            // 
            tsmiSpawnItemButton.Name = "tsmiSpawnItemButton";
            tsmiSpawnItemButton.Size = new System.Drawing.Size(225, 26);
            tsmiSpawnItemButton.Text = "Spawn";
            // 
            // tsmiSpawnNPC
            // 
            tsmiSpawnNPC.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripComboBox1, toolStripSeparator2, toolStripMenuItem1, toolStripMenuItem3, toolStripSeparator5, toolStripMenuItem2 });
            tsmiSpawnNPC.Name = "tsmiSpawnNPC";
            tsmiSpawnNPC.Size = new System.Drawing.Size(136, 26);
            tsmiSpawnNPC.Text = "NPC";
            // 
            // toolStripComboBox1
            // 
            toolStripComboBox1.Name = "toolStripComboBox1";
            toolStripComboBox1.Size = new System.Drawing.Size(121, 28);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(222, 6);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(225, 26);
            toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new System.Drawing.Size(225, 26);
            toolStripMenuItem3.Text = "toolStripMenuItem3";
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(222, 6);
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(225, 26);
            toolStripMenuItem2.Text = "toolStripMenuItem2";
            // 
            // tsmiSpawnObject
            // 
            tsmiSpawnObject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripComboBox2, toolStripSeparator3, toolStripMenuItem4, toolStripSeparator6, toolStripMenuItem5 });
            tsmiSpawnObject.Name = "tsmiSpawnObject";
            tsmiSpawnObject.Size = new System.Drawing.Size(136, 26);
            tsmiSpawnObject.Text = "Object";
            // 
            // toolStripComboBox2
            // 
            toolStripComboBox2.Name = "toolStripComboBox2";
            toolStripComboBox2.Size = new System.Drawing.Size(121, 28);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(222, 6);
            // 
            // toolStripMenuItem4
            // 
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            toolStripMenuItem4.Size = new System.Drawing.Size(225, 26);
            toolStripMenuItem4.Text = "toolStripMenuItem4";
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new System.Drawing.Size(222, 6);
            // 
            // toolStripMenuItem5
            // 
            toolStripMenuItem5.Name = "toolStripMenuItem5";
            toolStripMenuItem5.Size = new System.Drawing.Size(225, 26);
            toolStripMenuItem5.Text = "toolStripMenuItem5";
            // 
            // tsmiEditHereMenu
            // 
            tsmiEditHereMenu.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiEditHereMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiEditItem, tsmiEditNpc, tsmiEditObject, tsmiEditTile });
            tsmiEditHereMenu.Enabled = false;
            tsmiEditHereMenu.Name = "tsmiEditHereMenu";
            tsmiEditHereMenu.Size = new System.Drawing.Size(224, 26);
            tsmiEditHereMenu.Text = "Edit";
            // 
            // tsmiEditItem
            // 
            tsmiEditItem.Name = "tsmiEditItem";
            tsmiEditItem.Size = new System.Drawing.Size(136, 26);
            tsmiEditItem.Text = "Item";
            // 
            // tsmiEditNpc
            // 
            tsmiEditNpc.Enabled = false;
            tsmiEditNpc.Name = "tsmiEditNpc";
            tsmiEditNpc.Size = new System.Drawing.Size(136, 26);
            tsmiEditNpc.Text = "NPC";
            // 
            // tsmiEditObject
            // 
            tsmiEditObject.Enabled = false;
            tsmiEditObject.Name = "tsmiEditObject";
            tsmiEditObject.Size = new System.Drawing.Size(136, 26);
            tsmiEditObject.Text = "Object";
            // 
            // tsmiEditTile
            // 
            tsmiEditTile.Enabled = false;
            tsmiEditTile.Name = "tsmiEditTile";
            tsmiEditTile.Size = new System.Drawing.Size(136, 26);
            tsmiEditTile.Text = "Tile";
            // 
            // tsmiTeleportMenu
            // 
            tsmiTeleportMenu.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiTeleportMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiWaypointMenu, tsmiTeleportToClosest, tsmiTeleportToHere });
            tsmiTeleportMenu.Enabled = false;
            tsmiTeleportMenu.Name = "tsmiTeleportMenu";
            tsmiTeleportMenu.Size = new System.Drawing.Size(224, 26);
            tsmiTeleportMenu.Text = "Teleportation";
            // 
            // tsmiWaypointMenu
            // 
            tsmiWaypointMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiAddWaypoint, toolStripSeparator8, toolStripMenuItem6 });
            tsmiWaypointMenu.Name = "tsmiWaypointMenu";
            tsmiWaypointMenu.Size = new System.Drawing.Size(212, 26);
            tsmiWaypointMenu.Text = "Waypoints";
            // 
            // tsmiAddWaypoint
            // 
            tsmiAddWaypoint.Name = "tsmiAddWaypoint";
            tsmiAddWaypoint.Size = new System.Drawing.Size(164, 26);
            tsmiAddWaypoint.Text = "New";
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new System.Drawing.Size(161, 6);
            // 
            // toolStripMenuItem6
            // 
            toolStripMenuItem6.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMenuItem7 });
            toolStripMenuItem6.Name = "toolStripMenuItem6";
            toolStripMenuItem6.Size = new System.Drawing.Size(164, 26);
            toolStripMenuItem6.Text = "AWaypoint";
            // 
            // toolStripMenuItem7
            // 
            toolStripMenuItem7.Name = "toolStripMenuItem7";
            toolStripMenuItem7.Size = new System.Drawing.Size(146, 26);
            toolStripMenuItem7.Text = "Remove";
            // 
            // tsmiTeleportToClosest
            // 
            tsmiTeleportToClosest.Name = "tsmiTeleportToClosest";
            tsmiTeleportToClosest.Size = new System.Drawing.Size(212, 26);
            tsmiTeleportToClosest.Text = "To Closest...";
            // 
            // tsmiTeleportToHere
            // 
            tsmiTeleportToHere.Name = "tsmiTeleportToHere";
            tsmiTeleportToHere.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T;
            tsmiTeleportToHere.Size = new System.Drawing.Size(212, 26);
            tsmiTeleportToHere.Text = "Display XY";
            tsmiTeleportToHere.Click += tsmiItem_Click;
            // 
            // tsmiMiscMenu
            // 
            tsmiMiscMenu.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiMiscMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiChecksum, tsmiGenerateCT });
            tsmiMiscMenu.Name = "tsmiMiscMenu";
            tsmiMiscMenu.Size = new System.Drawing.Size(224, 26);
            tsmiMiscMenu.Text = "Misc";
            // 
            // tsmiChecksum
            // 
            tsmiChecksum.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiChecksum.Checked = true;
            tsmiChecksum.CheckOnClick = true;
            tsmiChecksum.CheckState = System.Windows.Forms.CheckState.Checked;
            tsmiChecksum.Name = "tsmiChecksum";
            tsmiChecksum.Size = new System.Drawing.Size(176, 26);
            tsmiChecksum.Text = "Checksum";
            tsmiChecksum.CheckedChanged += tsmiItem_CheckedChanged;
            // 
            // tsmiGenerateCT
            // 
            tsmiGenerateCT.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiGenerateCT.Name = "tsmiGenerateCT";
            tsmiGenerateCT.Size = new System.Drawing.Size(176, 26);
            tsmiGenerateCT.Text = "Generate .CT";
            tsmiGenerateCT.Click += tsmiItem_Click;
            // 
            // tsmiTeleport
            // 
            tsmiTeleport.Name = "tsmiTeleport";
            tsmiTeleport.Size = new System.Drawing.Size(210, 24);
            tsmiTeleport.Text = "Teleport";
            tsmiTeleport.Click += tsmiItem_Click;
            // 
            // tsmiDirection
            // 
            tsmiDirection.Name = "tsmiDirection";
            tsmiDirection.Size = new System.Drawing.Size(210, 24);
            tsmiDirection.Text = "Turn Towards";
            tsmiDirection.Click += tsmiItem_Click;
            // 
            // tsmiGetItems
            // 
            tsmiGetItems.Name = "tsmiGetItems";
            tsmiGetItems.Size = new System.Drawing.Size(210, 24);
            tsmiGetItems.Text = "Get Items";
            tsmiGetItems.Click += tsmiItem_Click;
            // 
            // tsmiSettings
            // 
            tsmiSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiFont });
            tsmiSettings.Name = "tsmiSettings";
            tsmiSettings.Size = new System.Drawing.Size(210, 24);
            tsmiSettings.Text = "Settings";
            // 
            // tsmiFont
            // 
            tsmiFont.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            tsmiFont.Name = "tsmiFont";
            tsmiFont.Size = new System.Drawing.Size(121, 26);
            tsmiFont.Text = "Font";
            tsmiFont.Click += tsmiItem_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new System.Drawing.Size(207, 6);
            // 
            // tsmiCloseMenu
            // 
            tsmiCloseMenu.Name = "tsmiCloseMenu";
            tsmiCloseMenu.Size = new System.Drawing.Size(210, 24);
            tsmiCloseMenu.Text = "Close Menu";
            tsmiCloseMenu.Click += tsmiItem_Click;
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.Color.FromArgb(226, 197, 153);
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new System.Windows.Forms.Padding(2);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(222, 275);
            panel1.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(222, 275);
            Controls.Add(panel1);
            Margin = new System.Windows.Forms.Padding(2);
            Name = "Form1";
            Text = "Form1";
            FormClosing += OnFormClosing;
            FormClosed += OnFormClosed;
            Load += Form1_Load;
            cmsMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip cmsMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiCloseMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem tsmiCraftingMenu;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem tsmiCheatMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiPassiveCheats;
        private System.Windows.Forms.ToolStripMenuItem tsmiNoInjuries;
        private System.Windows.Forms.ToolStripMenuItem tsmiNoCarryWeight;
        private System.Windows.Forms.ToolStripMenuItem tsmiNoNeeds;
        private System.Windows.Forms.ToolStripMenuItem tsmiTreeVision;
        private System.Windows.Forms.ToolStripMenuItem tsmiCannibalism;
        private System.Windows.Forms.ToolStripMenuItem tsmiFreezeNPC;
        private System.Windows.Forms.ToolStripMenuItem tsmiSpawn;
        private System.Windows.Forms.ToolStripMenuItem tsmiSpawnItem;
        private System.Windows.Forms.ToolStripComboBox tscbSpawnItemSelect;
        private System.Windows.Forms.ToolStripTextBox tstbSpawnItemQuantity;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiSpawnItemName;
        private System.Windows.Forms.ToolStripMenuItem tsmiSpawnItemType;
        private System.Windows.Forms.ToolStripMenuItem tsmiSpawnNPC;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem tsmiSpawnObject;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem tsmiFreezeTime;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem tsmiSpawnItemButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem20;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditorMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiCharacterEditor;
        private System.Windows.Forms.ToolStripMenuItem tsmiInventoryEditor;
        private System.Windows.Forms.ToolStripMenuItem tsmiItemEditor;
        private System.Windows.Forms.ToolStripMenuItem tsmiInjuryEditor;
        private System.Windows.Forms.ToolStripMenuItem tsmiSpawnMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiTeleportMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiWaypointMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddWaypoint;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem tsmiTeleportToClosest;
        private System.Windows.Forms.ToolStripMenuItem tsmiTeleportToHere;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditHereMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditNpc;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditObject;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditTile;
        private System.Windows.Forms.ToolStripMenuItem tsmiTeleport;
        private System.Windows.Forms.ToolStripMenuItem tsmiMiscMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiChecksum;
        private System.Windows.Forms.ToolStripMenuItem tsmiDirection;
        private System.Windows.Forms.ToolStripMenuItem tsmiGenerateCT;
        private System.Windows.Forms.ToolStripMenuItem tsmiGetItems;
        private System.Windows.Forms.ToolStripMenuItem tsmiSettings;
        private System.Windows.Forms.ToolStripMenuItem tsmiFont;
    }
}

