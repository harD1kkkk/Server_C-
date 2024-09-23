using System;
using System.Windows.Forms;

namespace Client
{
    public partial class ChatForm : Form
    {
        // Declare the UI components as fields
        private TextBox txtChat;
        private TextBox txtMessage;
        private Button btnSend;

        public ChatForm()
        {
            InitializeComponent();
            initializeComponent();
        }

        private void initializeComponent()
        {
            this.txtChat = new System.Windows.Forms.TextBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtChat
            // 
            this.txtChat.Location = new System.Drawing.Point(12, 12);
            this.txtChat.Multiline = true;
            this.txtChat.Name = "txtChat";
            this.txtChat.ReadOnly = true;
            this.txtChat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtChat.Size = new System.Drawing.Size(460, 300);
            this.txtChat.TabIndex = 0;
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(12, 320);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(340, 22);
            this.txtMessage.TabIndex = 1;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(360, 318);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(112, 26);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 361);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.txtChat);
            this.Name = "ChatForm";
            this.Text = "Chat";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ChatForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // Button click event to send a message
        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = txtMessage.Text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                txtChat.AppendText($"You: {message}\r\n");
                txtMessage.Clear(); // Clear the message box
            }
        }

        // Form closed event
        private void ChatForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Logic to handle when the chat form is closed
        }
    }
}
