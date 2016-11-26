Public Class User
    Inherits OriginalDataModel(Of User)
    Public Shared cachedList As ArrayList = New ArrayList()
    Public Shared cachedListIndex As ArrayList = New ArrayList()

    Public Overrides Function getIdFieldName() As String
        Return "userId"
    End Function

    Public Sub New(ByVal objectToClone As User)
        clone(objectToClone)
    End Sub

    Public Sub New()
        initialize("user.csv")
    End Sub

    Public Sub loadCachedListAndIndex()
        loadCachedList()
        Dim idFieldPos As Short = Array.IndexOf(attributeNames, idFieldName)
        For Each objUser As String() In cachedList
            cachedListIndex.Add(Integer.Parse(objUser(idFieldPos)))
        Next
    End Sub

    Public Sub loadCachedList()
        cachedList = lighterListAll()
    End Sub
End Class
