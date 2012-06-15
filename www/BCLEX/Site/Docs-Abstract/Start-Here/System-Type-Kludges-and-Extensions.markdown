# System Type Kludges and Extensions


# Kludges
To support a consistant library BclEx-Abstract reimplements the following dependecy .Net 4.0 types in .Net 3.5:

## System namespace

### Lazy&lt;T&gt;
Same Lazy&lt;T&gt; from .net40

# Extensions
These are additional types included in this library

## System namespace

### Lazy&lt;T, TMetadata&gt;
Lazy&lt;T&gt; with an additional Metadata property.
<table>
<tr>
	<th>name</th>
	<th>description</th>
</tr>
<tr>
	<td>Metadata</td>
	<td>Description here</td>
</tr>
</table>

### EnvironmentEx
<table>
<tr>
	<th>name</th>
	<th>description</th>
</tr>
<tr>
	<td>ApplicationID</td>
	<td>Description here</td>
</tr>
<tr>
	<td>DeploymentEnvironment</td>
	<td>Description here</td>
</tr>
<tr>
	<td>DevelopmentStage</td>
	<td>Description here</td>
</tr>
<tr>
	<td>OSVersionEx</td>
	<td>Description here</td>
</tr>
<tr>
	<td>NextID()</td>
	<td>Description here</td>
</tr>
</table>


### DevelopmentStage
Enum values based on the wiki [Software release life cycle](http://en.wikipedia.org/wiki/Software_release_life_cycle "Software release life cycle"). This is a type exposed from the EnvironmentEx type above.
<table>
<tr>
	<th>name</th>
	<th>description</th>
</tr>
<tr>
	<td>PreAlpha</td>
	<td>Description here</td>
</tr>
<tr>
	<td>Alpha</td>
	<td>Description here</td>
</tr>
<tr>
	<td>Beta</td>
	<td>Description here</td>
</tr>
<tr>
	<td>Release</td>
	<td>Description here</td>
</tr>
<tr>
	<td>ToCode()</td>
	<td>Description here</td>
</tr>
</table>


### DeploymentEnvironment
Enum values based on the wiki [Software_testing](http://en.wikipedia.org/wiki/Software_testing "Software testing"). This is a type exposed from the EnvironmentEx type above.
<table>
<tr>
	<th>name</th>
	<th>description</th>
</tr>
<tr>
	<td>ProofOfConcept</td>
	<td>Description here</td>
</tr>
<tr>
	<td>Local</td>
	<td>Description here</td>
</tr>
<tr>
	<td>Development</td>
	<td>Description here</td>
</tr>
<tr>
	<td>AlphaTesting</td>
	<td>Description here</td>
</tr>
<tr>
	<td>BetaTesting</td>
	<td>Description here</td>
</tr>
<tr>
	<td>Production</td>
	<td>Description here</td>
</tr>
<tr>
	<td>IncrementTag</td>
	<td>Description here</td>
</tr>
<tr>
	<td>ToShortName()</td>
	<td>Description here</td>
</tr>
<tr>
	<td>IsExternalDeployment()</td>
	<td>Description here</td>
</tr>
<tr>
	<td>ToCode()</td>
	<td>Description here</td>
</tr>
</table>
