﻿'Open VOGEL (www.openvogel.com)
'Open source software for aerodynamics
'Copyright (C) 2016 Guillermo Hazebrouck (guillermo.hazebrouck@openvogel.com)

'This program Is free software: you can redistribute it And/Or modify
'it under the terms Of the GNU General Public License As published by
'the Free Software Foundation, either version 3 Of the License, Or
'(at your option) any later version.

'This program Is distributed In the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty Of
'MERCHANTABILITY Or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License For more details.

'You should have received a copy Of the GNU General Public License
'along with this program.  If Not, see < http:  //www.gnu.org/licenses/>.

Imports MathTools.Algebra.EuclideanSpace
Imports System.Xml
Imports AeroTools.VisualModel.IO

Namespace CalculationModel.SimulationTools

    Public Enum HistogramType As Byte

        FlutterTest = 0
        SteadyTest = 1

    End Enum

    Public Class StepData

        Public Property Time As Double
        Public Property Damping As Double
        Public Property Velocity As New EVector3

    End Class

    Public Interface IAeroelasticHistogram

        ReadOnly Property Type As HistogramType
        ReadOnly Property State(TimeStep As Integer) As StepData

        Sub Generate(Velocity As EVector3, TimeStep As Double, Steps As Integer)
        Function Clone() As IAeroelasticHistogram

        Sub SaveToXML(ByRef writer As XmlWriter)
        Sub ReadFromXML(ByRef reader As XmlReader)

        Event ValueChanged()

    End Interface

    Public Class HistogramTools

        Public Shared Function GetInstance(Type As HistogramType) As IAeroelasticHistogram

            Select Case Type

                Case HistogramType.FlutterTest
                    Return New FlutterTestHistogram
                Case Else
                    Return Nothing

            End Select

        End Function

    End Class

    Public Class FlutterTestHistogram

        Implements IAeroelasticHistogram

        Public ReadOnly Property Type As HistogramType Implements IAeroelasticHistogram.Type
            Get
                Return HistogramType.FlutterTest
            End Get
        End Property

        Private _HyperDamping As Double

        Public Property HyperDamping As Double
            Set(value As Double)
                _HyperDamping = value
                RaiseEvent ValueChanged()
            End Set
            Get
                Return _HyperDamping
            End Get
        End Property

        Private _HyperDampingSpan As Double

        Public Property HyperDampingSpan As Double
            Set(value As Double)
                _HyperDampingSpan = value
                RaiseEvent ValueChanged()
            End Set
            Get
                Return _HyperDampingSpan
            End Get
        End Property

        Private _NormalDamping As Double

        Public Property NormalDamping As Double
            Set(value As Double)
                _NormalDamping = value
                RaiseEvent ValueChanged()
            End Set
            Get
                Return _NormalDamping
            End Get
        End Property

        Private _GustX As Double

        Public Property GustX As Double
            Set(value As Double)
                _GustX = value
                RaiseEvent ValueChanged()
            End Set
            Get
                Return _GustX
            End Get
        End Property

        Private _GustY As Double

        Public Property GustY As Double
            Set(value As Double)
                _GustY = value
                RaiseEvent ValueChanged()
            End Set
            Get
                Return _GustY
            End Get
        End Property

        Private _GustZ As Double

        Public Property GustZ As Double
            Set(value As Double)
                _GustZ = value
                RaiseEvent ValueChanged()
            End Set
            Get
                Return _GustZ
            End Get
        End Property

        Private _GustSpan As Double

        Public Property GustSpan As Double
            Set(value As Double)
                _GustSpan = value
                RaiseEvent ValueChanged()
            End Set
            Get
                Return _GustSpan
            End Get
        End Property

        Private _Steps As New List(Of StepData)

        Public Event ValueChanged() Implements IAeroelasticHistogram.ValueChanged

        Public ReadOnly Property State(TimeStep As Integer) As StepData Implements IAeroelasticHistogram.State
            Get
                Return _Steps(TimeStep)
            End Get
        End Property

        Public Sub Generate(Velocity As EVector3, TimeStep As Double, Steps As Integer) Implements IAeroelasticHistogram.Generate

            _Steps.Clear()

            Dim i As Integer = 0

            Dim sd0 As New StepData
            sd0.Damping = HyperDamping
            sd0.Velocity.Assign(Velocity)
            sd0.Time = i * TimeStep
            _Steps.Add(sd0)

            While _Steps(i).Time < HyperDampingSpan And i < Steps

                Dim sd1 As New StepData

                sd1.Damping = HyperDamping
                sd1.Velocity.Assign(Velocity)
                sd1.Time = i * TimeStep
                i += 1

                _Steps.Add(sd1)

            End While

            Dim t As Double = 0.0

            While _Steps(i).Time < HyperDampingSpan + GustSpan And i < Steps

                Dim sd As New StepData

                sd.Damping = NormalDamping
                sd.Velocity.Assign(Velocity)

                Dim f As Double = 0.5 * (Math.Sin(2 * Math.PI * (t / GustSpan - 0.25)) + 1)

                sd.Velocity.X += f * GustX
                sd.Velocity.Y += f * GustY
                sd.Velocity.Z += f * GustZ

                t += TimeStep

                sd.Time = _Steps(_Steps.Count - 1).Time + TimeStep

                i += 1

                _Steps.Add(sd)

            End While

            While i < Steps

                Dim sd As New StepData
                sd.Damping = NormalDamping
                sd.Velocity.Assign(Velocity)
                sd.Time = i * TimeStep
                i += 1

                _Steps.Add(sd)

            End While

        End Sub

        Public Function Clone() As IAeroelasticHistogram Implements IAeroelasticHistogram.Clone

            Dim clonedHistogram As FlutterTestHistogram = New FlutterTestHistogram
            clonedHistogram.GustX = GustX
            clonedHistogram.GustY = GustY
            clonedHistogram.GustZ = GustZ
            clonedHistogram.GustSpan = GustSpan
            clonedHistogram.HyperDamping = HyperDamping
            clonedHistogram.HyperDampingSpan = HyperDampingSpan
            clonedHistogram.NormalDamping = NormalDamping

            Return clonedHistogram

        End Function

        Public Sub New()

            HyperDamping = 6
            HyperDampingSpan = 0.55
            NormalDamping = 0.01
            GustX = 0
            GustY = 0
            GustZ = 0
            GustSpan = 0.4

        End Sub

        Sub SaveToXML(ByRef writer As XmlWriter) Implements IAeroelasticHistogram.SaveToXML

            writer.WriteStartElement("FlutterTestHistogram")
            writer.WriteAttributeString("HyperDamping", String.Format("{0}", HyperDamping))
            writer.WriteAttributeString("NormalDamping", String.Format("{0}", NormalDamping))
            writer.WriteAttributeString("HyperDampingSpan", String.Format("{0}", HyperDampingSpan))
            writer.WriteAttributeString("GustIntensityX", String.Format("{0}", GustX))
            writer.WriteAttributeString("GustIntensityY", String.Format("{0}", GustY))
            writer.WriteAttributeString("GustIntensityZ", String.Format("{0}", GustZ))
            writer.WriteAttributeString("GustSpan", String.Format("{0}", GustSpan))
            writer.WriteEndElement()

        End Sub

        Public Sub ReadFromXML(ByRef reader As XmlReader) Implements IAeroelasticHistogram.ReadFromXML

            While reader.Read

                If reader.NodeType = XmlNodeType.Element Then

                    Select Case reader.Name

                        Case "FlutterTestHistogram"

                            HyperDamping = IOXML.ReadDouble(reader, "HyperDamping", 1.0)
                            NormalDamping = IOXML.ReadDouble(reader, "NormalDamping", 0.01)
                            HyperDampingSpan = IOXML.ReadDouble(reader, "HyperDampingSpan", 1.0)
                            GustX = IOXML.ReadDouble(reader, "GustIntensityX", 0.0)
                            GustY = IOXML.ReadDouble(reader, "GustIntensityY", 0.0)
                            GustZ = IOXML.ReadDouble(reader, "GustIntensityZ", 0.0)
                            GustSpan = IOXML.ReadDouble(reader, "GustSpan", 0.0)

                    End Select

                End If

            End While

        End Sub

    End Class

End Namespace
