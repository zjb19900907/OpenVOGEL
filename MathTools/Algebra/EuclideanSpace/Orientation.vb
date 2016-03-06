﻿'Copyright (C) 2016 Guillermo Hazebrouck

Namespace Algebra.EuclideanSpace

    ''' <summary>
    ''' Represents an orientation in Euler angles
    ''' </summary>
    ''' <remarks></remarks>
    Public Class OrientationCoordinates

        Public Psi As Double
        Public Tita As Double
        Public Fi As Double

        Public Sub SetToCero()
            Me.Psi = 0
            Me.Tita = 0
            Me.Fi = 0
        End Sub

        Public Function ToRadians() As OrientationCoordinates

            Dim NuevaOrientacion As New OrientationCoordinates
            Dim Conversion As Double = Math.PI / 180
            NuevaOrientacion.Psi = Me.Psi * Conversion
            NuevaOrientacion.Tita = Me.Tita * Conversion
            NuevaOrientacion.Fi = Me.Fi * Conversion
            Return NuevaOrientacion

        End Function

        Public Sub Assign(ByVal Ori As OrientationCoordinates)

            Me.Fi = Ori.Fi
            Me.Psi = Ori.Psi
            Me.Tita = Ori.Tita

        End Sub

    End Class

End Namespace