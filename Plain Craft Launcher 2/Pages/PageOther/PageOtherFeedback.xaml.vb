﻿Public Class PageOtherFeedback

    Private Shadows IsLoaded As Boolean = False
    Private Sub PageOtherFeedback_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        '重复加载部分
        PanBack.ScrollToHome()

        '非重复加载部分
        If IsLoaded Then Exit Sub
        IsLoaded = True

        RefreshList()

    End Sub

    Public Sub RefreshList()
        PanContent.Children.Clear()
        PanContent.Children.Add(New MyLoading() With {.Text = "加载反馈列表中……", .HorizontalAlignment = HorizontalAlignment.Center})
        Dim list As JArray
        Dim thread As Thread = RunInNewThread(Sub()
                                                  list = NetGetCodeByRequestRetry("https://api.github.com/repos/Hex-Dragon/PCL2/issues", IsJson:=True, UseBrowserUserAgent:=True)
                                              End Sub, "GetFeedbackList")
        While thread.IsAlive
            Thread.Sleep(100)
        End While
        PanContent.Children.Clear()
        If List Is Nothing Then Exit Sub

        For Each i As JObject In list
            Dim item As MyListItem = New MyListItem With {.Title = i("title").ToString(), .Info = "反馈者：" & i("user")("login").ToString() & " | 时间：" & Date.Parse(i("created_at").ToString()).ToLocalTime().ToString(), .Logo = $"https://github.com/{i("user")("login").ToString()}.png", .Type = MyListItem.CheckType.Clickable, .Tag = i}
            AddHandler item.Click, AddressOf SeeDetail
            PanContent.Children.Add(item)
        Next
    End Sub

    Private Sub SeeDetail(sender As Object, e As MouseButtonEventArgs)
        Dim data As JObject = sender.Tag
        MyMsgBox(data("body").ToString(), data("user")("login").ToString() & " | " & data("title").ToString(), Button2:="查看详情", Button2Action:=Sub()
                                                                                                                                                   OpenWebsite(data("html_url"))
                                                                                                                                               End Sub)
    End Sub


    Private Sub Feedback_Click(sender As Object, e As MouseButtonEventArgs)
        PageOtherLeft.TryFeedback()
    End Sub
End Class
