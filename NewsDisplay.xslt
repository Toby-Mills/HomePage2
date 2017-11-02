<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<rss>
			<channel>
				<xsl:for-each select="/rss/channel/item">
					<xsl:sort select="pubDateSort" order="descending"/>
					<item>
						<link>
						<xsl:value-of select="link"/>
					</link>
					<title>
						<xsl:value-of select="title"/>
					</title>
					<description>
						<xsl:value-of select="description"/>
					</description>
						<pubDate>
							<xsl:value-of select="pubDate"/>
						</pubDate>
					</item>
				</xsl:for-each>
			</channel>
		</rss>
	</xsl:template>
</xsl:stylesheet>
