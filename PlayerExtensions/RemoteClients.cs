﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace Mpdn.PlayerExtensions
{
    public partial class RemoteClients : Form
    {
        #region Variables
        public AcmPlug MainRemote;
        #endregion

        #region Delegates
        private delegate void UpdateDgRowDelegate(string[] row);
        private delegate void ClearGridDelegate();
        #endregion

        #region Constructor
        public RemoteClients(AcmPlug control)
        {
            InitializeComponent();
            MainRemote = control;
            this.Load += RemoteClients_Load;
        }
        #endregion

        #region Internal Methods
        internal void ForceUpdate()
        {
            PopulateGrid();
        }
        #endregion

        #region Private Methods
        void RemoteClients_Load(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        private void PopulateGrid()
        {
            dgMainGrid.Invoke(new ClearGridDelegate(ClearGrid));
            foreach(var item in MainRemote.GetClients)
            {
                try
                {
                    IPEndPoint remoteIpEndPoint = item.Value.RemoteEndPoint as IPEndPoint;
                    string[] tmpRow = { item.Key.ToString(), remoteIpEndPoint.Address + ":" + remoteIpEndPoint.Port };
                    dgMainGrid.Invoke(new UpdateDgRowDelegate(AddRow), new object[]{tmpRow});
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error " + ex.ToString());
                }
            } 

        }

        private void ClearGrid()
        {
            dgMainGrid.Rows.Clear();
        }

        private void AddRow(string[] row)
        {
            dgMainGrid.Rows.Add(row);
            dgMainGrid.Refresh();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void dgMainGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (dgMainGrid.SelectedRows.Count != 0)
                btnDisconnect.Enabled = true;
            else
                btnDisconnect.Enabled = false;
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if(dgMainGrid.SelectedRows.Count == 1)
            {
                var clientGuid = dgMainGrid.SelectedRows[0].Cells[0].Value.ToString();
                MainRemote.DisconnectClient(clientGuid);
            }
        }
    }
}
