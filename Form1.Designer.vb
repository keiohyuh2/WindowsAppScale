<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class form1
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextScaleCurrent = New System.Windows.Forms.TextBox()
        Me.TimerScaleScan = New System.Windows.Forms.Timer(Me.components)
        Me.ButtonTimer = New System.Windows.Forms.Button()
        Me.ButtonEnd = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TextScaleConfirm = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(58, 48)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(126, 19)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "現在の総重量"
        '
        'TextScaleCurrent
        '
        Me.TextScaleCurrent.BackColor = System.Drawing.SystemColors.Info
        Me.TextScaleCurrent.Enabled = False
        Me.TextScaleCurrent.Font = New System.Drawing.Font("MS UI Gothic", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextScaleCurrent.ForeColor = System.Drawing.SystemColors.InfoText
        Me.TextScaleCurrent.Location = New System.Drawing.Point(62, 70)
        Me.TextScaleCurrent.Name = "TextScaleCurrent"
        Me.TextScaleCurrent.Size = New System.Drawing.Size(150, 28)
        Me.TextScaleCurrent.TabIndex = 1
        '
        'TimerScaleScan
        '
        '
        'ButtonTimer
        '
        Me.ButtonTimer.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonTimer.Location = New System.Drawing.Point(321, 57)
        Me.ButtonTimer.Name = "ButtonTimer"
        Me.ButtonTimer.Size = New System.Drawing.Size(200, 50)
        Me.ButtonTimer.TabIndex = 2
        Me.ButtonTimer.Text = "計量モニター開始"
        Me.ButtonTimer.UseVisualStyleBackColor = True
        '
        'ButtonEnd
        '
        Me.ButtonEnd.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonEnd.Location = New System.Drawing.Point(321, 321)
        Me.ButtonEnd.Name = "ButtonEnd"
        Me.ButtonEnd.Size = New System.Drawing.Size(200, 50)
        Me.ButtonEnd.TabIndex = 3
        Me.ButtonEnd.Text = "終了"
        Me.ButtonEnd.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button1.Location = New System.Drawing.Point(321, 149)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(200, 50)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "確定"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'TextScaleConfirm
        '
        Me.TextScaleConfirm.BackColor = System.Drawing.SystemColors.Info
        Me.TextScaleConfirm.Enabled = False
        Me.TextScaleConfirm.Font = New System.Drawing.Font("MS UI Gothic", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextScaleConfirm.ForeColor = System.Drawing.Color.Red
        Me.TextScaleConfirm.Location = New System.Drawing.Point(62, 162)
        Me.TextScaleConfirm.Name = "TextScaleConfirm"
        Me.TextScaleConfirm.Size = New System.Drawing.Size(150, 28)
        Me.TextScaleConfirm.TabIndex = 5
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(58, 140)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(109, 19)
        Me.Label2.TabIndex = 6
        Me.Label2.Text = "確定総重量"
        '
        'form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(684, 461)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TextScaleConfirm)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.ButtonEnd)
        Me.Controls.Add(Me.ButtonTimer)
        Me.Controls.Add(Me.TextScaleCurrent)
        Me.Controls.Add(Me.Label1)
        Me.Name = "form1"
        Me.Text = "計量モニター画面"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents TextScaleCurrent As TextBox
    Friend WithEvents TimerScaleScan As Timer
    Friend WithEvents ButtonTimer As Button
    Friend WithEvents ButtonEnd As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents TextScaleConfirm As TextBox
    Friend WithEvents Label2 As Label
End Class
